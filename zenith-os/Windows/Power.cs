using Cosmos.System.Graphics;
using zenithos.Controls;

namespace zenithos.Windows
{
    internal class Power : Window
    {
        public Button shutdownButton, restartButton;
        public Power() : base(0, 0, 140, 90, "Power...", Kernel.defFont, false)
        {
            x = (int)Kernel.canv.Mode.Width / 2 - 140 / 2;
            y = (int)Kernel.canv.Mode.Height / 2 - 90 / 2;
            shutdownButton = new Button("Shut Down", 20, 20, Kernel.textColDark, Kernel.defFont);
            restartButton = new Button("Reboot",20,50,Kernel.textColDark,Kernel.defFont);

            controls.Add(restartButton);
            controls.Add(shutdownButton);
        }

        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);

            if(shutdownButton.clickedOnce)
            {
                Cosmos.System.Power.Shutdown();
            }

            if (restartButton.clickedOnce)
            {
                Cosmos.System.Power.Reboot();
            }
        }
    }
}
