using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SoundFader.utils;

namespace SoundFader.models
{
    internal class DeviceFaderSettingModel : SoundFaderSettingModel
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public bool Default { get; set; } = false;
        public int Direction { get; set; }

        [JsonIgnore]
        public Direction DirectionT
        {
            get => (Direction)this.Direction;
            set => this.Direction = (int)value;
        }
    }
}
