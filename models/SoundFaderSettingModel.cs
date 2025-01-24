using SoundFader.controllers;
using Newtonsoft.Json;

namespace SoundFader.models
{
    internal abstract class SoundFaderSettingModel
    {
        public int Mode { get; set; } = 0;
        public int Fader { get; set; } = 0;
        public int Duration { get; set; } = 500;
        public float Target { get; set; } = 100;
        public bool DisplayName { get; set; } = true;

        [JsonIgnore]
        public FaderActionMode ModeT {
            get => (FaderActionMode)this.Mode;
            set => this.Mode = (int)value;
        }

        [JsonIgnore]
        public Fader FaderT
        {
            get => (Fader)this.Fader;
            set => this.Fader = (int)value;
        }
    }
}
