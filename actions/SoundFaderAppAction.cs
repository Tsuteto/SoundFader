using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundFader.actionbase;
using SoundFader.controllers;
using SoundFader.utils;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace SoundFader.actions
{
    [ActionUuid(Uuid = "jp.tsuteto.soundfader.app-fader")]
    class SoundFaderAppAction : BaseCustomSdActionWithSettingsModel<models.AppFaderSettingModel>
    {

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            try
            {
                SoundFaderApp.PerformFading(this.Manager, args.context, this.SettingsModel);
            }
            catch (Exception ex)
            {
                await Manager.LogMessageAsync(args.context, ex.ToString());
            }
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            try
            {
                //await Manager.LogMessageAsync(args.context, $"list: {SoundFaderApp.DumpAppList()}");
                var app = SoundFaderApp.FindApp(this.SettingsModel.AppId);
                if (app != null)
                {
                    this.SettingsModel.AppName = app.Name;
                    this.SettingsModel.FilePath = app.FilePath;
                    this.SettingsModel.ProcName = app.ProcName;
                    this.SettingsModel.System = app.System;
                    await Manager.LogMessageAsync(args.context, $"Specified app: {this.SettingsModel.AppName} ({this.SettingsModel.AppId})");
                }

                if (this.SettingsModel.ModeT != FaderActionMode.TOGGLE)
                {
                    this.SettingsModel.FaderT =
                        this.SettingsModel.ModeT == FaderActionMode.IN ? FadeDir.IN : FadeDir.OUT;
                }
                await Manager.SetSettingsAsync(args.context, this.SettingsModel);

                var data = SoundFaderApp.GetAppIcon(this.SettingsModel);
                await Manager.SetImageFromDataUriAsync(args.context, data);
            }
            catch (Exception ex)
            {
                await Manager.LogMessageAsync(args.context, ex.ToString());
            }
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);

            try
            {
                var data = SoundFaderApp.GetAppIcon(this.SettingsModel);
                await Manager.SetImageFromDataUriAsync(args.context, data);
            }
            catch (Exception ex)
            {
                await Manager.LogMessageAsync(args.context, $"Error while setting icon:\n{ex}");
            }
        }

        public override async Task OnPropertyInspectorDidAppear(StreamDeckEventPayload args)
        {
            try
            {
                var list = SoundFaderApp.GetAppList();
                await Manager.SendToPropertyInspectorAsync(args.context, JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                await Manager.LogMessageAsync(args.context, $"Error while obtaining app list:\n{ex}");
            }
        }
    }
}
