using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace SoundFader.actionbase;

public abstract class BaseCustomSdActionWithSettingsModel<T> : BaseStreamDeckActionWithSettingsModel<T>
{

    public override Task OnDidReceiveSettings(StreamDeckEventPayload args)
    {
        SetModelProperties(args);
        return Task.CompletedTask;
    }

    public override Task OnWillAppear(StreamDeckEventPayload args)
    {
        SetModelProperties(args);
        return Task.CompletedTask;
    }

    protected new void SetModelProperties(StreamDeckEventPayload args)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();
        PropertyInfo[] array = properties;
        foreach (PropertyInfo propertyInfo in array)
        {
            if (args.payload != null && args.payload.settings != null && args.payload.settings.settingsModel != null && args.PayloadSettingsHasProperty(propertyInfo.Name) && !Attribute.IsDefined(propertyInfo, typeof(JsonIgnoreAttribute)))
            {
                object payloadSettingsValue = args.GetCustomPayloadSettingsValue(propertyInfo.Name);
                object value = Convert.ChangeType(payloadSettingsValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(SettingsModel, value);
            }
        }
    }
}