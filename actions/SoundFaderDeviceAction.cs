using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundFader.actionbase;
using SoundFader.controllers;
using SoundFader.utils;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace SoundFader.actions
{
    [ActionUuid(Uuid = "jp.tsuteto.soundfader.device-fader")]
    class SoundFaderAction : BaseCustomSdActionWithSettingsModel<models.DeviceFaderSettingModel>
    {

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            try
            {
                SoundFaderDevice.PerformFading(Manager, args.context, this.SettingsModel);
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
                //await Manager.LogMessageAsync(args.context, $"list: {SoundFaderDevice.DumpDeviceList()}");
                var info = SoundFaderDevice.FindDevice(this.SettingsModel.DeviceId);
                if (info != null)
                {
                    this.SettingsModel.DeviceName = info.Name;
                    this.SettingsModel.DirectionT = info.Direction;
                    this.SettingsModel.Default = info.Default;
                    await Manager.LogMessageAsync(args.context, $"Specified device: {this.SettingsModel.DeviceName} ({this.SettingsModel.DeviceId})");
                }
                this.SettingsModel.FaderT =
                        this.SettingsModel.ModeT == FaderActionMode.IN ? Fader.IN : Fader.OUT;

                await Manager.SetSettingsAsync(args.context, this.SettingsModel);

                var data = SoundFaderDevice.GetDeviceIcon(this.SettingsModel);
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
                var data = SoundFaderDevice.GetDeviceIcon(this.SettingsModel);
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
                var list = SoundFaderDevice.GetDeviceList();
                await Manager.SendToPropertyInspectorAsync(args.context, JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                await Manager.LogMessageAsync(args.context, $"Error while obtaining app list:\n{ex}");
            }
        }

    }
}
