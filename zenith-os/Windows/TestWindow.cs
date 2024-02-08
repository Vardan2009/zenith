
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using zenithos.Controls;

namespace zenithos.Windows
{
    internal class TestWindow : Window
    {
        public Label welcomeLabel = new("Welcome to ZenithOS!",20,20,Kernel.defFont,Color.White);
        public Button clickButton = new("Click Me!", 20, 50, Color.Green, Kernel.defFont, 10);
        public Label clickLabel = new("You Clicked it 0 times",20,100,Kernel.defFont,Color.White);
        int times;
        public TestWindow() : base(20, 20, 200, 300, "Test Window", Kernel.defFont)
        {
          controls.Add(welcomeLabel); controls.Add(clickButton); controls.Add(clickLabel);
        }

        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);
            if(clickButton.clickedOnce)
            {
                times++;
                clickLabel.Text = $"You Clicked it {times} times";
            }
        }
    }
}
