using IL2CPU.API.Attribs;
using System;
using zenithos.Controls;

namespace zenithos.Windows
{
    internal class Error : Window
    {
        public Label text;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.error.bmp")]
        static byte[] logoBytes;

        static int GetLineCount(string text)
        {
            string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Length;
        }
        static string GetLongestString(string str1, string str2)
        {
            if (str1.Length > str2.Length)
            {
                return str1;
            }
            else
            {
                return str2;
            }
        }

        public Error(string title,string err) : base(300,300, 200, 100, title, Kernel.defFont, false)
        {
            logo = new Cosmos.System.Graphics.Bitmap(logoBytes);
            w = Kernel.defFont.Width * GetLongestString(err,title).Length + 20;
            h = Kernel.defFont.Height * GetLineCount(err) + 20;
            x = (int)Kernel.canv.Mode.Width / 2 - w / 2;
            y = (int)Kernel.canv.Mode.Height / 2 - h / 2;
            text = new Label(err, 10, 10, font, Kernel.textColDark);
            controls.Add(text);
        }
    }
}
