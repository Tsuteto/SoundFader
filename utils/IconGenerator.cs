using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using SoundFader.controllers;

namespace SoundFader.utils
{
    internal class IconGenerator
    {
        const int CANVAS_WIDTH = 144;
        const int CANVAS_HEIGHT = 144;
        const float FONT_SIZE = 20;
        const float FONT_SIZE_MIN = 12;

        public static Bitmap GenerateIconForApp(
            string appPath, bool system, FadeDir fader, string appName, float targetVol, bool displayName)
        {
            Bitmap canvas = new(CANVAS_WIDTH, CANVAS_HEIGHT);
            Graphics g = Graphics.FromImage(canvas);

            // App icon
            if (system)
            {
                appPath = @"::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            }
            Bitmap appIcon = IconHelper.GetHighResolutionIcon(appPath, 128, 128);
            if (appIcon != null)
            {
                g.DrawImage(appIcon, 8, 8, 72, 72);
            }
            else
            {
                g.DrawImage(Image.FromFile("images/unknown.png"), 8, 8, 72, 72);
            }

            // Action icon
            string iconPath = GetActionIconPath(fader);
            g.DrawImage(Image.FromFile(iconPath), CANVAS_WIDTH - 8 - 96, 56, 96, 48);

            // Target volume
            if (fader == FadeDir.IN)
            {
                DrawVolume(g, targetVol);
            }

            // App name
            if (displayName)
            {
                DrawName(g, appName);
            }

            return canvas;
        }

        public static Bitmap GenerateIconForDevice(
            FadeDir fader, string deviceIconPath, string deviceName, float targetVol, bool displayName)
        {
            Bitmap canvas = new(CANVAS_WIDTH, CANVAS_HEIGHT);
            Graphics g = Graphics.FromImage(canvas);

            // Device icon
            g.DrawImage(Image.FromFile(deviceIconPath), 8, 8, 72, 72);

            // Action icon
            string actionIconPath = GetActionIconPath(fader);
            g.DrawImage(Image.FromFile(actionIconPath), CANVAS_WIDTH - 8 - 96, 56, 96, 48);

            // Target volume
            if (fader == FadeDir.IN)
            {
                DrawVolume(g, targetVol);
            }

            // Device name
            if (displayName)
            {
                DrawName(g, deviceName);
            }

            return canvas;
        }

        private static void DrawVolume(Graphics g, float volume)
        {
            using GraphicsPath path = new();

            StringFormat format = new()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            };
            path.AddString(Math.Round(volume).ToString(), new FontFamily("Segoe UI"), (int)FontStyle.Regular, 32,
                new PointF(0, 0), format);

            var rect = path.GetBounds();

            using Matrix matrix = new();
            matrix.Translate(0, -rect.Height, MatrixOrder.Append);
            matrix.Rotate(-30, MatrixOrder.Append);
            matrix.Translate(CANVAS_WIDTH - 14, 45, MatrixOrder.Append);
            path.Transform(matrix);

            g.FillPath(Brushes.White, path);
        }

        private static void DrawName(Graphics g, string content)
        {
            StringFormat format = new()
            {
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.LineLimit
            };

            var fontFamily = new FontFamily("Segoe UI");
            SizeF size = g.MeasureString(content, new Font(fontFamily, FONT_SIZE, FontStyle.Bold));

            bool isLongText = size.Width > CANVAS_WIDTH;
            float ratio = isLongText ? CANVAS_WIDTH / size.Width : 1;
            if (FONT_SIZE * ratio < FONT_SIZE_MIN)
            {
                var actualFont = new Font(fontFamily, FONT_SIZE_MIN, FontStyle.Bold);
                float posX = isLongText ? 0 : (CANVAS_WIDTH - size.Width) / 2;
                float posY = CANVAS_HEIGHT - 45 + (FONT_SIZE_MIN / 2) * 1.33f;
                g.DrawString(content, actualFont, Brushes.White,
                    new RectangleF(posX, posY, CANVAS_WIDTH, size.Height * (FONT_SIZE_MIN / FONT_SIZE)), format);
            }
            else
            {
                var actualFont = new Font(fontFamily, FONT_SIZE * ratio, FontStyle.Bold);
                float posX = isLongText ? 0 : (CANVAS_WIDTH - size.Width) / 2;
                float posY = CANVAS_HEIGHT - 45 + size.Height * (1 - ratio) / 2;
                g.DrawString(content, actualFont, Brushes.White, posX, posY);
            }
        }

        private static string GetActionIconPath(FadeDir mode)
        {
            return mode == FadeDir.OUT ? "images/fade-out.png" : "images/fade-in.png";
        }
    }
}
