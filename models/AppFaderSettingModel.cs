using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SoundFader.utils;

namespace SoundFader.models
{
    internal class AppFaderSettingModel : SoundFaderSettingModel
    {
        public string AppId { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public bool System { get; set; } = false;
        public string FilePath { get; set; }
        public string ProcName { get; set; }

    }
}
