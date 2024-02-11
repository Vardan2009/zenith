using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;
using static Cosmos.HAL.BlockDevice.ATA_PIO;
using static System.Net.Mime.MediaTypeNames;

namespace zenithos.Controls
{
    internal class InputField : Control
    {
        public string Value;
        public int x, y, width,padding;
        VBECanvas canv;
        Font font;
        int _pX, _pY;
        bool focused = false;
        bool lmD;
        public bool submittedOnce;
        bool showCursor;
        int frames;
        int framesToUpdateCursor = 50;
        public InputField(int x,int y,int width,Font font,int padding)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            canv = Kernel.canv;
            this.font = font;
            this.padding = padding;
        }
        static string GetLastXCharacters(string str, int x)
        {
            if (str.Length <= x)
            {
                return str; // Return the whole string if it's not long enough
            }
            else
            {
                return str.Substring(str.Length - x); // Get the last x characters
            }
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
          
           
            focused = MouseInBounds(mX, mY, !mD && lmD);

            if(frames>framesToUpdateCursor)
            {
                frames = 0;
                showCursor = !showCursor;
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
            canv.DrawString(GetLastXCharacters(Value,(int)MathF.Round(width/font.Width)-(focused?1:0))+(showCursor&&focused?"_":""), font, Color.White,x+pX+padding,y+pY+padding);
            lmD = mD;
            if(focused)
                frames++;
        }

        public bool MouseInBounds(int mX, int mY,bool condition)
        {
            if (mX >= x + _pX && mX <= x + width + (padding * 2) + _pX &&
                mY >= y + _pY && mY <= y + (padding * 2) + _pY+font.Height)
            {
                if(condition)
                    return true;
            }
            else
            {
                if (condition)
                    return false;
            }

            return focused;
        }
    }
}
