using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundFader.models;
using SoundFader.utils;
using StreamDeckLib;

namespace SoundFader.controllers
{
    internal class SoundFaderApp : SoundFaderCommon
    {
        private static List<AppInfo> appList = [];
        private readonly static List<string> appsOccupied = [];

        public static async void PerformFading(ConnectionManager Manager, string context, AppFaderSettingModel settingsModel)
        {
            lock (appsOccupied)
            {
                if (appsOccupied.Contains(settingsModel.AppId))
                {
                    return;
                }
                appsOccupied.Add(settingsModel.AppId);
            }

            Fader fader = settingsModel.FaderT;
            Action<float> setAudioVolume;
            float initial;

            if (!settingsModel.System)
            {
                var session = AudioHelper.GetSession(settingsModel.FilePath, settingsModel.ProcName);
                if (session != null)
                {
                    var appVolume = session.SimpleAudioVolume;
                    setAudioVolume = vol => appVolume.Volume = vol;
                    initial = appVolume.Volume;
                }
                else
                {
                    await Manager.LogMessageAsync(context, $"{settingsModel.ProcName} is currently not running or has no audio session");
                    await Manager.ShowAlertAsync(context);
                    lock (appsOccupied)
                    {
                        appsOccupied.Remove(settingsModel.AppId);
                    }
                    return;
                }
            }
            else
            {
                var systemVolume = AudioHelper.GetSystemVolume();
                setAudioVolume = vol => systemVolume.MasterVolumeLevelScalar = vol;
                initial = systemVolume.MasterVolumeLevelScalar;
            }

            _ = Task.Run(() => FadingTask(
                    Manager, context, fader,
                    initial, settingsModel.Duration, settingsModel.Target, setAudioVolume)
                ).ContinueWith(async _ =>
                {
                    if (settingsModel.ModeT == FaderActionMode.TOGGLE)
                    {
                        if (fader == Fader.OUT)
                        {
                            settingsModel.Target = initial * 100f;
                        }
                        var nextFader = settingsModel.FaderT switch
                        {
                            Fader.OUT => Fader.IN,
                            Fader.IN => Fader.OUT,
                            _ => Fader.OUT
                        };
                        settingsModel.FaderT = nextFader;
                        await Manager.SetSettingsAsync(context, settingsModel);

                        var data = GetAppIcon(settingsModel);
                        await Manager.SetImageFromDataUriAsync(context, data);
                    }

                    lock (appsOccupied)
                    {
                        appsOccupied.Remove(settingsModel.AppId);
                    }
                });
        }

        public static string GetAppIcon(AppFaderSettingModel settings)
        {
            if (string.IsNullOrEmpty(settings.AppId))
            {
                return null;
            }

            if (settings.FilePath != null)
            {
                using var appIcon = IconGenerator.GenerateIconForApp(
                    settings.FilePath, settings.System, settings.FaderT,
                    settings.AppName, settings.Target, settings.DisplayName);
                return GetDataUri(appIcon);
            }
            return null;
        }

        public static List<AppInfo> GetAppList()
        {
            var list = AudioHelper.GetAudioAppList();
            appList = list;
            return list;
        }

        public static AppInfo FindApp(string appId) => appList.Find(a => a.Id == appId);

        public static string DumpAppList() => JsonConvert.SerializeObject(appList, Formatting.Indented);
    }
}
