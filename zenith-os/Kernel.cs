using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;
using zenithos.Windows;
using Sys = Cosmos.System;

namespace zenithos
{
    public class Kernel : Sys.Kernel
    {
        public static Color bgCol = Color.FromArgb(37, 45, 51);
        public static Color highlightCol = Color.FromArgb(16, 136, 227);
        public static Color textColLight = Color.FromArgb(29, 30, 31);
        public static Color textColDark = Color.FromArgb(200, 200, 204);
        public static VBECanvas canv;
        public static Font defFont;
        public static List<Window> windows = new();
        static List<Application> applications = new();
        List<Button> applicationsButtons = new();
        Button mainButton;
        public static int activeIndex = -1;
        bool mainBar;
        [ManifestResourceStream(ResourceName = "zenithos.Resource.blue.bmp")]
        static byte[] bgImage;

        Bitmap bg;

        void DrawTopbar()
        {
            canv.DrawFilledRectangle(bgCol, 0, 0, (int)canv.Mode.Width, 30);
            mainButton.Update(0, 0);
        }

        void DrawMainBar()
        {
            canv.DrawFilledRectangle(bgCol, 0, 30, 300, 300);
            for(int i =0;i<applicationsButtons.Count;i++)
            {
                applicationsButtons[i].Update(0, 30);
                if (applicationsButtons[i].clickedOnce)
                {
                    Window instance = applications[i].constructor();
                    windows.Add(instance);
                    mainBar = false;
                    break;
                }
            }
        }

        class Application
        {
            public Func<Window> constructor;
            public string name;
            public Application(Func<Window> constructor, string name)
            {
                this.constructor = constructor;
                this.name = name;
            }
        }

        protected override void BeforeRun()
        {
            canv = new VBECanvas();
            defFont = PCScreenFont.Default;
            MouseManager.ScreenWidth = canv.Mode.Width;
            MouseManager.ScreenHeight = canv.Mode.Height;
            MouseManager.X = MouseManager.ScreenWidth / 2;
            MouseManager.Y = MouseManager.ScreenHeight / 2;

            mainButton = new Button("Zenith", 0, 0, textColDark, defFont);
            bg = new Bitmap(bgImage);

            applications.Add(new Application(() => new Calc(), "Calculator"));
            applications.Add(new Application(() => new TestWindow(), "Test Window"));

           
            for (int i = 0; i < applications.Count; i++)
            {
                applicationsButtons.Add(new Button(applications[i].name, 20, 20 + i * 30, textColDark, defFont));
            }
        }

        public void DrawCursor(uint x, uint y, Color col)
        {
            int xPos = (int)x;
            int yPos = (int)y;

            for (int i = xPos - 5; i <= xPos + 5; i++)
            {
                if (i >= 0 && i < canv.Mode.Width && yPos >= 0 && yPos < canv.Mode.Width)
                    canv.DrawPoint(col, i, yPos);
            }

            for (int j = yPos - 5; j <= yPos + 5; j++)
            {
                if (j >= 0 && j < canv.Mode.Width && xPos >= 0 && xPos < canv.Mode.Width)
                    canv.DrawPoint(col, xPos, j);
            }
        }
        
        protected override void Run()
        {
            canv.Clear();
            canv.DrawImage(bg, 0, 0);
            DrawTopbar();
          
            if(mainButton.clickedOnce)
            {
                mainBar = !mainBar;
            }

            

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            int dmx = MouseManager.DeltaX;
            int dmy = MouseManager.DeltaY;
            for(int i =0;i<windows.Count;i++)
            {
                if(i != activeIndex)
                    windows[i].Update(canv,mx,my, MouseManager.MouseState == MouseState.Left, dmx, dmy);
            }
            if(activeIndex != -1 && windows.Count > 0)
                windows[activeIndex].Update(canv, mx, my, MouseManager.MouseState == MouseState.Left, dmx, dmy);

            if (mainBar) DrawMainBar();

            DrawCursor(MouseManager.X,MouseManager.Y, Color.White);
            canv.Display();
        }
    }
}
