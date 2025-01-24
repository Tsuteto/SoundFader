using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundFader.models;
using SoundFader.utils;
using StreamDeckLib;

namespace SoundFader.controllers
{
    internal class SoundFaderDevice : SoundFaderCommon
    {
        private static List<DeviceInfo> deviceList = [];
        private readonly static List<string> devicesOccupied = [];

        public static async void PerformFading(ConnectionManager Manager, string context, DeviceFaderSettingModel settingsModel)
        {
            lock (devicesOccupied)
            {
                if (devicesOccupied.Contains(settingsModel.DeviceId))
                {
                    return;
                }
                devicesOccupied.Add(settingsModel.DeviceId);
            }

            Fader fader = settingsModel.FaderT;
            Action<float> setAudioVolume;
            float initial;

            if (!settingsModel.Default)
            {
                var device = AudioHelper.GetDevice(settingsModel.DeviceId);
                if (device == null)
                {
                    await Manager.ShowAlertAsync(context);
                    await Manager.LogMessageAsync(context, $"Device is inactive or not found: {settingsModel.DeviceName}");
                    lock (devicesOccupied)
                    {
                        devicesOccupied.Remove(settingsModel.DeviceId);
                    }
                    return;
                }
                setAudioVolume = vol => device.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
                initial = device.AudioEndpointVolume.MasterVolumeLevelScalar;
            }
            else
            {
                var device = AudioHelper.GetDefaultDevice((Direction)settingsModel.Direction);
                setAudioVolume = vol => device.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
                initial = device.AudioEndpointVolume.MasterVolumeLevelScalar;
            }

            _ = Task.Run(() => SoundFaderCommon.FadingTask(
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

                        var data = SoundFaderDevice.GetDeviceIcon(settingsModel);
                        await Manager.SetImageFromDataUriAsync(context, data);
                    }

                    lock (devicesOccupied)
                    {
                        devicesOccupied.Remove(settingsModel.DeviceId);
                    }
                });

        }

        public static string GetDeviceIcon(DeviceFaderSettingModel settings)
        {
            if (string.IsNullOrEmpty(settings.DeviceId))
            {
                return null;
            }

            string deviceIconPath = settings.DirectionT == Direction.OUT ? "images/device-speaker.png" : "images/device-mic.png";
            using var icon = IconGenerator.GenerateIconForDevice(
                settings.FaderT, deviceIconPath, settings.DeviceName, settings.Target, settings.DisplayName);
            return GetDataUri(icon);
        }

        public static List<DeviceInfo> GetDeviceList()
        {
            var list = AudioHelper.GetAudioDeviceList();
            deviceList = list;
            return list;
        }

        public static DeviceInfo FindDevice(string deviceId) => deviceList.Find(d => d.Id == deviceId);

        public static string DumpDeviceList() => JsonConvert.SerializeObject(deviceList, Formatting.Indented);
    }
}
