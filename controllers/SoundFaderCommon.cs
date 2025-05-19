using System;
using System.Collections.Generic;
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

    internal enum FadeDir
    {
        OUT, IN
    }

    internal enum BendingType
    {
        POW, LOG
    }

    internal class SoundFaderCommon
    {
        protected delegate double Fader(double b, double x);

        protected static readonly IFadingBehavior FadingLinear = new FadingLinear();
        protected static readonly Dictionary<BendingType, IFadingBehavior> FadingBehaviors = new() {
            { BendingType.POW, new FadingPower()},
            { BendingType.LOG, new FadingLogarithmic()},
        };

        public static async void FadingTask(
            ConnectionManager Manager, string context, FadeDir dir,
            float initial, int duration, float fadeInTargetVol,
            int bendingOut, int bendingIn,
            BendingType bendingTypeOut, BendingType bendingTypeIn,
            Action<float> setAudioVolume)
        {
            try
            {
                float target;
                float bending;
                IFadingBehavior fadingMethod;
                Fader fader;

                if (dir == FadeDir.OUT)
                {
                    target = 0f;
                    bending = bendingOut + 1.0f;
                    fadingMethod = bending <= 1.0 ? FadingLinear : FadingBehaviors[bendingTypeOut];
                    fader = fadingMethod.FadeOut;
                }
                else // FadeDir.IN
                {
                    target = fadeInTargetVol / 100f;
                    bending = bendingIn + 1.0f;
                    fadingMethod = bending <= 1.0 ? FadingLinear : FadingBehaviors[bendingTypeIn];
                    fader = fadingMethod.FadeIn;
                }

                Stopwatch sw = Stopwatch.StartNew();
                float elapsed;
                while ((elapsed = (float)sw.Elapsed.TotalMilliseconds) <= duration)
                {
                    var vol = (target - initial) * fader(bending, elapsed / duration) + initial;
                    //await Manager.LogMessageAsync(context, vol.ToString());
                    setAudioVolume((float)Math.Ceiling(vol * 100) / 100);
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
