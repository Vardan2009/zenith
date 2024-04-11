using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace zenithos.Controls
{
    public class Button : Control
    {
        public string Text { get; set; }
        public Color color,textColor = Kernel.textColDark;
        public Font font;
        public int padding;
        public bool clicked, clickedOnce,hovered;
        bool lmD;
        VBECanvas canv;
        int _pX, _pY;
        Bitmap image;
        int height;
        int imagewidth;

        int fixedWidth = -1;
        bool usingFixedWidth;


        public Button(string text, int x, int y, Color color, Font font, int padding = 5,Bitmap image = null,int fixedWidth = -1)
        {
            Text = text;
            this.x = x;
            this.y = y;
            this.color = color;
            this.font = font;
            this.padding = padding;
            canv = Kernel.canv;
            this.image = image;

            usingFixedWidth = fixedWidth != -1;
            this.fixedWidth = fixedWidth;

            height = image != null ? (font.Height >= (int)image.Height ? (int)image.Height : font.Height) : font.Height;
            imagewidth = image != null ? (int)image.Width + 5 : 0;
        }

        public Button(string text, int x, int y, Color color,Color textCol, Font font, int padding = 5, Bitmap image = null,int fixedWidth = -1) : this(text, x, y, color, font, padding, image,fixedWidth)
        {
            textColor = textCol;
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
            hovered = Hovered(mX, mY);


            int cw = usingFixedWidth ? fixedWidth : font.Width * Text.Length + padding * 2 + imagewidth;
            if (clicked)
            {
                
                canv.DrawFilledRectangle(Color.FromArgb(color.R / 2, color.G / 2, color.B / 2), x + pX, y + pY,cw, height + padding * 2);
            }
            else if (hovered)
            {
                canv.DrawFilledRectangle(color, x+pX, y + pY, cw, height + padding * 2);
            }
            else
            {
                canv.DrawRectangle(color, x + pX, y + pY, cw, height + padding * 2-1);
            }

            canv.DrawString(Text, font, textColor, x + padding + pX + imagewidth, y + padding + pY);

            if (image!=null)
            {
                canv.DrawImageAlpha(image, x + padding/2 + pX, y + padding + pY);
            }

            lmD = mD;
        }

        public bool Hovered(int mX, int mY)
        {
            if (mX >= x + _pX && mX <= x + (font.Width * Text.Length) + (padding * 2) + _pX + imagewidth &&
                mY >= y + _pY && mY <= y + height + (padding * 2) + _pY)
            {
                
                    return true;
            }

            return false;
        }
        public bool Clicked(int mX, int mY, bool mD)
        {
            if (Hovered(mX,mY))
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
