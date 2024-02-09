using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;
using zenithos.Windows;
using System.Linq;
using Sys = Cosmos.System;
using Cosmos.Core.Memory;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Audio;

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
        static byte[] bgBytes;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.cur.bmp")]
        static byte[] curBytes;

      
        [ManifestResourceStream(ResourceName = "zenithos.Resource.startup.wav")]
        static byte[] sampleAudioBytes;

        public static Bitmap bg,cursor;

        void DrawTopbar()
        {
            canv.DrawFilledRectangle(bgCol, 0, 0, (int)canv.Mode.Width, 30);
            mainButton.Update(0, 0);
            string time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");
            canv.DrawString(time, defFont, textColDark, (int)canv.Mode.Width - 20 - defFont.Width * time.Length, 10);
            canv.DrawString(activeIndex != -1 && windows.Count != 0 && activeIndex < windows.Count ? windows[activeIndex].title:"",defFont,textColDark,70,10);
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
            public Bitmap logo;
            public string name;
            public Application(Func<Window> constructor, string name,Bitmap logo)
            {
                this.constructor = constructor;
                this.name = name;
                this.logo = logo;
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
            bg = new Bitmap(bgBytes);
            cursor = new Bitmap(curBytes);
            

            applications.Add(new Application(() => new Calc(), "Calculator",new Calc().logo));
            applications.Add(new Application(() => new TestWindow(), "Test Window",new TestWindow().logo));
            applications.Add(new Application(() => new Windows.Power(), "Power...",new Windows.Power().logo));
           
            for (int i = 0; i < applications.Count; i++)
            {
                applicationsButtons.Add(new Button(applications[i].name, 20, 20 + i * 30, textColDark, defFont, 5, applications[i].logo));
            }
            var mixer = new AudioMixer();
            var audioStream = MemoryAudioStream.FromWave(sampleAudioBytes);
            try
            {
                var driver = AC97.Initialize(bufferSize: 4096);
            }
            catch(Exception ex)
            {
                windows.Add(new Error("Audio Driver Initialization Error", "Failed to initialize AC97 driver!\nMessage: "+ex.Message));
            }

        }

        public void DrawCursor(uint x, uint y)
        {
            int xPos = (int)x;
            int yPos = (int)y;

            if(yPos>canv.Mode.Height- 16)
            {
                yPos = (int)canv.Mode.Height - 16;
            }

            /*for (int i = xPos - 5; i <= xPos + 5; i++)
            {
                if (i >= 0 && i < canv.Mode.Width && yPos >= 0 && yPos < canv.Mode.Width)
                    canv.DrawPoint(col, i, yPos);
            }

            for (int j = yPos - 5; j <= yPos + 5; j++)
            {
                if (j >= 0 && j < canv.Mode.Width && xPos >= 0 && xPos < canv.Mode.Width)
                    canv.DrawPoint(col, xPos, j);
            }*/
            canv.DrawImageAlpha(cursor, xPos, yPos);
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

            DrawCursor(MouseManager.X,MouseManager.Y);
            canv.Display();
            Heap.Collect();
        }
    }
}
