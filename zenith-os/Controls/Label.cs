using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace zenithos.Controls
{
    public class Label : Control
    {
        public string Text;
        VBECanvas canv;
        Font font;
        Color color;
        public Label(string text,int x,int y,Font font,Color color)
        {
            Text = text;
            this.x = x;
            this.y = y;
            this.font = font;
            this.color = color;
            canv = Kernel.canv;
        }

        public override void Update(int pX,int pY)
        {
            if (!Visible) return;
            canv.DrawString(Text, font, color, x + pX, y + pY);
        }
    }
}
