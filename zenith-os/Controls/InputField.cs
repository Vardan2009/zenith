using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;
using static Cosmos.HAL.BlockDevice.ATA_PIO;
using static System.Net.Mime.MediaTypeNames;

namespace zenithos.Controls
{
    internal class InputField : Control
    {
        public string Value;
        int x, y, width,padding;
        VBECanvas canv;
        Font font;
        int _pX, _pY;
        bool focused = false;
        bool lmD;
        public bool submittedOnce;
        public InputField(int x,int y,int width,Font font,int padding)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            canv = Kernel.canv;
            this.font = font;
            this.padding = padding;
        }
        public override void Update(int pX, int pY)
        {
            if (!Visible) return;
            if (submittedOnce) submittedOnce = false;
            _pX = pX;
            _pY = pY;
            int mX = (int)MouseManager.X;
            int mY = (int)MouseManager.Y;
            bool mD = MouseManager.MouseState == MouseState.Left;
          
            if (!mD && lmD)
            {
                focused = MouseInBounds(mX, mY);
            }

            if(focused)
            {
                if(KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    if(key.Key == ConsoleKeyEx.Backspace)
                    {
                       Value = Value.Remove(Value.Length - 1);
                    }
                    else if(key.Key == ConsoleKeyEx.Enter)
                    {
                        focused = false;
                        submittedOnce = true;
                    }
                    else
                    {
                        Value += key.KeyChar;
                    }
                }
                canv.DrawFilledRectangle(Color.Gray, x + pX, y + pY, width + padding * 2, font.Height + padding * 2);
            }

            canv.DrawRectangle(Color.White,x+pX, y+pY, width + padding * 2, font.Height + padding * 2);
            canv.DrawString(Value, font, Color.White,x+pX+padding,y+pY+padding);
            lmD = mD;
        }

        public bool MouseInBounds(int mX, int mY)
        {
            if (mX >= x + _pX && mX <= x + width + (padding * 2) + _pX &&
                mY >= y + _pY && mY <= y + (padding * 2) + _pY)
            {
               
                    return true;
            }

            return false;
        }
    }
}
