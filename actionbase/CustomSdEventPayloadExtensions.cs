using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StreamDeckLib.Messages;

namespace SoundFader.actionbase
{
    public static class CustomSdEventPayloadExtensions
    {
        public static object GetCustomPayloadSettingsValue(this StreamDeckEventPayload obj, string propertyName)
        {
            if (obj.PayloadSettingsHasProperty(propertyName))
            {
                return obj.payload.settings.settingsModel[propertyName].Value;
            }

            return null;
        }
    }
}
