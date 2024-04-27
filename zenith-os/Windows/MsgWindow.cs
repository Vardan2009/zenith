using IL2CPU.API.Attribs;
using System;
using zenithos.Controls;

namespace zenithos.Windows
{
    public enum MsgType
    {
        Info, Error
    }

    internal class MsgWindow : Window
    {
        public Label text;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.error.bmp")]
        static byte[] errLogoBytes;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.info.bmp")]
        static byte[] infoLogoBytes;

       

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

        public MsgWindow(string title,string err,MsgType type = MsgType.Info) : base(300,300, 200, 100, title, Kernel.defFont, false)
        {
            switch(type)
            {
                case MsgType.Info:
                    logo = new Cosmos.System.Graphics.Bitmap(infoLogoBytes);
                    break;
                case MsgType.Error:
                    logo = new Cosmos.System.Graphics.Bitmap(errLogoBytes);
                    break;
            }
            w = Kernel.defFont.Width * GetLongestString(err,title).Length + 20;
            h = Kernel.defFont.Height * GetLineCount(err) + 20;
            x = (int)Kernel.canv.Mode.Width / 2 - w / 2;
            y = (int)Kernel.canv.Mode.Height / 2 - h / 2;
            text = new Label(err, 10, 10, font, Kernel.textColDark);
            controls.Add(text);
        }
    }
}
