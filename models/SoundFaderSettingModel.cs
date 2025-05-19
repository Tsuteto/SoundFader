using SoundFader.controllers;
using Newtonsoft.Json;
using System;

namespace SoundFader.models
{
    internal abstract class SoundFaderSettingModel
    {
        public int Mode { get; set; } = 0;
        public int Fader { get; set; } = 0;
        public int Duration { get; set; } = 500;
        public float Target { get; set; } = 100;
        public int BendingOut { get; set; } = 0;
        public int BendingIn { get; set; } = 0;
        public string BendingTypeOut { get; set; } = BendingType.POW.ToString();
        public string BendingTypeIn { get; set; } = BendingType.POW.ToString();
        public bool DisplayName { get; set; } = true;

        [JsonIgnore]
        public FaderActionMode ModeT {
            get => (FaderActionMode)this.Mode;
            set => this.Mode = (int)value;
        }

        [JsonIgnore]
        public FadeDir FaderT
        {
            get => (FadeDir)this.Fader;
            set => this.Fader = (int)value;
        }

        [JsonIgnore]
        public BendingType BendingTypeOutT
        {
            get => (BendingType)Enum.Parse(typeof(BendingType), this.BendingTypeOut);
            set => this.BendingTypeOut = value.ToString();
        }

        [JsonIgnore]
        public BendingType BendingTypeInT
        {
            get => (BendingType)Enum.Parse(typeof(BendingType), this.BendingTypeIn);
            set => this.BendingTypeIn = value.ToString();
        }
    }
}
