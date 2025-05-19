using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StreamDeckLib;

namespace SoundFader.controllers
{
    internal class FadingLinear : IFadingBehavior
    {
        public double FadeOut(double b, double x) => x;
        public double FadeIn(double b, double x) => x;
    }

    internal class FadingPower : IFadingBehavior
    {
        public double FadeOut(double b, double x) => 1 - Math.Pow(1 - x, b / 5.0);
        public double FadeIn(double b, double x) => Math.Pow(x, b / 5.0);
    }

    internal class FadingLogarithmic : IFadingBehavior
    {
        public double FadeOut(double b, double x) => Compute(b, x);
        public double FadeIn(double b, double x) => Compute(1.0 / b, x);

        private double Compute(double b, double x)
        {
            double p3 = Math.Pow(b, 2.5);
            double inner = (1.0 / p3) + ((p3 - 1.0) / p3) * x;
            double logBase = p3;
            double result = -Math.Log(inner) / Math.Log(logBase);
            return 1 - result;
        }
    }
    internal interface IFadingBehavior
    {
        double FadeOut(double b, double x);
        double FadeIn(double b, double x);
    }
}
