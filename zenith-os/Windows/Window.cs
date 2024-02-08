using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;

namespace zenithos.Windows
{
    public class Window
    {
        public int x, y;
        public int w, h;
        public string title;
        public Font font;
        public bool dragging;
        public bool startDrag;
        public bool resizing;
        const int window_titlebarsize = 30;
        bool lmD;
        public List<Control> controls = new();
        public bool resizable = false;
        public Button closeButton;
        public Window(int x, int y, int w, int h, string title, Font font,bool resizable = false)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.title = title;
            this.font = font;
            this.resizable = resizable;
            closeButton = new Button("X", w - 20 - font.Width,2, Color.Red, font);
        }

        public virtual void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            if (Clicked(mX, mY, mD && !lmD))
            {
                Kernel.activeIndex = Kernel.windows.FindIndex(x => x == this);
                dragging = true;
            }
            if (ClickedResize(mX, mY, mD && !lmD) && resizable)
            {
                resizing = true;
            }

            closeButton.x = w - 20 - font.Width;

            if (closeButton.clickedOnce)
            {
                Kernel.activeIndex = -1;
                Kernel.windows.Remove(this);
            }

            if (resizing)
            {
                w += dmX;
                h += dmY;
                if (w <= font.Width * title.Length + 20)
                {
                    w = font.Width * title.Length + 20;
                }
                if (!mD)
                {
                    resizing = false;
                }
            }

            if(Kernel.activeIndex == Kernel.windows.FindIndex(x => x == this) && Kernel.activeIndex != -1)
            {
                canv.DrawFilledRectangle(Kernel.highlightCol, x, y, w, window_titlebarsize);
                canv.DrawString(title, font, Kernel.textColLight, x + 10, y + 10);
            }
            else
            {
                canv.DrawFilledRectangle(Kernel.bgCol, x, y, w, window_titlebarsize);
                canv.DrawRectangle(Kernel.highlightCol, x, y, w, window_titlebarsize);
                canv.DrawString(title, font, Kernel.highlightCol, x + 10, y + 10);
            }

            if (dragging)
            {
                x += dmX;
                y += dmY;
                if (y <= 0)
                {
                    y = 0;
                }
                if (x <= 0)
                {
                    x = 0;
                }
                if (x >= canv.Mode.Width - w)
                {
                    x = (int)canv.Mode.Width - w;
                }
                if (y >= canv.Mode.Height - h - window_titlebarsize)
                {
                    y = (int)canv.Mode.Height - h - window_titlebarsize;
                }
              
                if (!mD)
                {
                    dragging = false;
                }
            }
          
           
            canv.DrawFilledRectangle(Kernel.bgCol, x, y + window_titlebarsize, w, h);
            if(resizable)
                canv.DrawFilledRectangle(Color.Gray, x + w - 20, y + h - 20 + window_titlebarsize, 20, 20);
            canv.DrawRectangle(Color.White, x, y + window_titlebarsize, w, h);

            foreach(Control c in controls)
            {
                c.Update(x, y + window_titlebarsize);
            }
            closeButton.Update(x, y);

            lmD = mD;
        }

        public bool Clicked(int mX, int mY, bool mD)
        {
            if (mX >= x && mX <= x + w &&
                mY >= y && mY <= y + window_titlebarsize)
            {
                if (mD)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ClickedResize(int mX, int mY, bool mD)
        {
            if (mX >= x + w - 20 && mX <= x + w &&
                mY >= y + h - 20 && mY <= y + h + window_titlebarsize)
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
