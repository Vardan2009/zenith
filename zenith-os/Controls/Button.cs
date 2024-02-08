using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace zenithos.Controls
{
    public class Button : Control
    {
        public string Text { get; set; }
        public Color color;
        public Font font;
        public int padding;
        public bool clicked, clickedOnce;
        bool mD, lmD;
        VBECanvas canv;
        int _pX, _pY;
        public Button(string text, int x, int y, Color color, Font font, int padding = 5)
        {
            Text = text;
            this.x = x;
            this.y = y;
            this.color = color;
            this.font = font;
            this.padding = padding;
            canv = Kernel.canv;
        }
        public override void Update(int pX, int pY)
        {
            if (!Visible) return;
            _pX = pX;
            _pY = pY;
            int mX = (int)MouseManager.X;
            int mY = (int)MouseManager.Y;
            bool mD = MouseManager.MouseState == MouseState.Left;
            clicked = Clicked(mX, mY, mD);
            clickedOnce = Clicked(mX, mY, !mD && lmD);
            if (clickedOnce)
            {
                canv.DrawFilledRectangle(color, x+pX, y + pY, font.Width * Text.Length + padding * 2, font.Height + padding * 2);
            }
            else if (clicked)
            {
                canv.DrawFilledRectangle(Color.FromArgb(color.R / 2, color.G / 2, color.B / 2), x + pX, y + pY, font.Width * Text.Length + padding * 2, font.Height + padding * 2);
                canv.DrawString(Text, font, color, x + padding+pX, y + padding+pY);
            }
            else
            {
                canv.DrawRectangle(color, x + pX, y + pY, font.Width * Text.Length + padding * 2, font.Height + padding * 2);
                canv.DrawString(Text, font, color, x + padding+pX, y + padding+pY);
            }
            lmD = mD;
        }
        public bool Clicked(int mX, int mY, bool mD)
        {
            if (mX >= x+_pX && mX <= x + (font.Width * Text.Length) + (padding * 2)+_pX &&
                mY >= y+_pY && mY <= y + font.Height + (padding * 2) + _pY)
            {
                if (mD)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
