using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAudio.CoreAudioApi;

namespace SoundFader.utils
{
    internal static class AudioHelper
    {
        internal const string APP_ID_SYSTEM = "__SYSTEMSOUND__";
        internal static readonly string DEVICE_ID_DEFAULT_OUT = "__DEFAULT_OUT__";
        internal static readonly string DEVICE_ID_DEFAULT_IN = "__DEFAULT_IN__";

        internal static List<AppInfo> GetAudioAppList()
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var sessions = device.AudioSessionManager.Sessions;
            List<AppInfo> appList = [];

            appList.Add(new AppInfo { Name = "System", FilePath = "", ProcName = "", Id = APP_ID_SYSTEM, System = true });

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];

                if (session.GetProcessID != 0)
                {
                    try
                    {
                        var process = Process.GetProcessById((int)session.GetProcessID);
                        var procName = process.ProcessName;
                        var fileName = process.MainModule?.FileName;
                        var appName = procName;
                        if (fileName != null)
                        {
                            var versionFileInfo = FileVersionInfo.GetVersionInfo(fileName);
                            if (versionFileInfo != null)
                            {
                                appName = versionFileInfo.ProductName;
                                if (string.IsNullOrEmpty(appName))
                                {
                                    appName = process.ProcessName;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(process.MainWindowTitle))
                        {
                            appName = process.MainWindowTitle;
                        }
                        appList.Add(new AppInfo { Name = appName, FilePath = fileName, ProcName = procName, Id = fileName ?? procName, System = false });
                    }
                    catch { }
                }
            }
            return appList.Distinct().ToList();
        }

        internal static AudioSessionControl GetSession(string filePath, string procName)
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var sessions = device.AudioSessionManager.Sessions;

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session.GetProcessID != 0)
                {
                    try
                    {
                        var process = Process.GetProcessById((int)session.GetProcessID);
                        if (process.MainModule != null && filePath != null && process.MainModule.FileName.Equals(filePath)
                            || process.ProcessName.Equals(procName))
                        {
                            return session;
                        }
                    }
                    catch { }
                }
            }
            return null;
        }

        internal static List<DeviceInfo> GetAudioDeviceList()
        {
            List<DeviceInfo> list = [
                new() {
                    Id = DEVICE_ID_DEFAULT_OUT,
                    Name = "Default",
                    Default = true,
                    Direction = Direction.OUT
                },
                new() {
                    Id = DEVICE_ID_DEFAULT_IN,
                    Name = "Default",
                    Default = true,
                    Direction = Direction.IN
                }
            ];

            using var enumerator = new MMDeviceEnumerator();

            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                list.Add(new DeviceInfo
                {
                    Id = endpoint.ID,
                    Name = endpoint.FriendlyName,
                    Default = false,
                    Direction = Direction.OUT
                });
            }

            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                list.Add(new DeviceInfo
                {
                    Id = endpoint.ID,
                    Name = endpoint.FriendlyName,
                    Default = false,
                    Direction = Direction.IN
                });
            }

            return list;
        }

        internal static MMDevice GetDevice(string id)
        {
            using var enumerator = new MMDeviceEnumerator();
            return enumerator.GetDevice(id);
        }

        internal static MMDevice GetDefaultDevice(Direction dir)
        {
            using var enumerator = new MMDeviceEnumerator();
            DataFlow dataFlow;
            if (dir == Direction.OUT)
            {
                dataFlow = DataFlow.Render;
            }
            else
            {
                dataFlow = DataFlow.Capture;
            }
            return enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia);
        }


        //internal static AudioSessionControl GetSystemSession()
        //{
        //    var deviceEnumerator = new MMDeviceEnumerator();
        //    var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        //    var sessions = device.AudioSessionManager.Sessions;

        //    for (int i = 0; i < sessions.Count; i++)
        //    {
        //        if (sessions[i].IsSystemSoundsSession)
        //        {
        //            return sessions[i];
        //        }
        //    }
        //    return null;
        //}

        internal static AudioEndpointVolume GetSystemVolume()
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return device.AudioEndpointVolume;
        }
    }

    public class AppInfo
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string ProcName { get; set; }
        public bool System { get; set; }
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not AppInfo other)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    public class DeviceInfo
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public string Id { get; set; }
        public Direction Direction { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not DeviceInfo other)
            {
                return false;
            }
            return this.Id.Equals(other.Id) && this.Direction == other.Direction;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Direction.GetHashCode();
        }
    }

    public enum Direction
    {
        OUT, IN
    }
}
