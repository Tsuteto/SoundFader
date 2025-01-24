using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using StreamDeckLib;

namespace SoundFader.controllers
{
    internal enum FaderActionMode
    {
        TOGGLE, OUT, IN
    }

    internal enum Fader
    {
        OUT, IN
    }

    internal class SoundFaderCommon
    {
        public static async void FadingTask(
            ConnectionManager Manager, string context, Fader fader,
            float initial, int duration, float fadeInTargetVol,
            Action<float> setAudioVolume)
        {
            try
            {
                float target = fader == Fader.OUT ? 0f : fadeInTargetVol / 100f;

                Stopwatch sw = Stopwatch.StartNew();
                float elapsed;
                while ((elapsed = (float)sw.Elapsed.TotalMilliseconds) <= duration)
                {
                    var vol = (target - initial) * (elapsed / duration) + initial;
                    //await Manager.LogMessageAsync(context, vol.ToString());
                    setAudioVolume(vol);
                    Thread.Sleep(20);
                }
                setAudioVolume(target);
            }
            catch (Exception ex)
            {
                await Manager.LogMessageAsync(context, ex.ToString());
            }
        }

        public static string GetDataUri(Bitmap image)
        {
            using MemoryStream stream = new();
            image.Save(stream, ImageFormat.Png);
            byte[] bytes = stream.ToArray();

            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
