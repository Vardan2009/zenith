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
using Cosmos.Core.Memory;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Audio;
using System.Threading;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;

namespace zenithos
{
    public class Kernel : Sys.Kernel
    {

        public static string version = "v0.2";

        public static Color bgCol = Color.FromArgb(31, 32, 33);
        public static Color mainCol = Color.FromArgb(57, 64, 69);
        public static Color highlightCol = Color.FromArgb(16, 136, 227);
        public static Color textColLight = Color.FromArgb(59, 71, 79);
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

        [ManifestResourceStream(ResourceName ="zenithos.Resource.zenith.bmp")]
        static byte[] logoBytes;
      
        [ManifestResourceStream(ResourceName = "zenithos.Resource.startup.wav")]
        static byte[] sampleAudioBytes;

        public static Bitmap bg,cursor,logo;

        CosmosVFS fs = new CosmosVFS();

        void DrawTopbar()
        {
            canv.DrawFilledRectangle(bgCol, 0, 0, (int)canv.Mode.Width, 30);
            mainButton.Update(0, 0);
            string time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");
            canv.DrawString(time, defFont, textColDark, (int)canv.Mode.Width - 20 - defFont.Width * time.Length, 10);
            canv.DrawString(activeIndex != -1 && windows.Count != 0 && activeIndex < windows.Count ? windows[activeIndex].title:"",defFont,textColDark,170,8);
        }

        void DrawMainBar()
        {
            canv.DrawFilledRectangle(bgCol, 10, 40, 300, applicationsButtons.Count*50+40);
            canv.DrawString("Welcome to Zenith!", defFont, textColDark, 40, 70 - defFont.Height);
            for(int i =0;i<applicationsButtons.Count;i++)
            {
                applicationsButtons[i].Update(10, 40);
                if (applicationsButtons[i].clickedOnce)
                {
                    Window instance = applications[i].constructor();
                    int mx = (int)MouseManager.X;
                    int my = (int)MouseManager.Y;
                    int dmx = MouseManager.DeltaX;
                    int dmy = MouseManager.DeltaY;
                    instance.Start(canv, mx, my, MouseManager.MouseState == MouseState.Left, dmx, dmy);
                    windows.Add(instance);
                    mainBar = false;
                    break;
                }
            }
           
        }

        public static void ShowMessage(string content,string title="Message",MsgType type = MsgType.Info)
        {
            windows.Add(new MsgWindow(title,content,type));
            activeIndex = windows.Count - 1;
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

        public static void HandleFatalError(Exception e)
        {
            canv.Clear(Color.DarkSlateBlue);
            Thread.Sleep(10);
            canv.Display();
            string[] lines = {
                 $"--- ZENITH OS {version}",
                 "",
                 $"The system has encountered an uncaught fatal exception",
                 "","Message: "
                 ,e.Message,"","Click any key to reboot"
            };

            int y = 10; 

            foreach (string line in lines)
            {
                canv.DrawString(line, PCScreenFont.Default, Color.White, 10, y);
                y += PCScreenFont.Default.Height+5;
                Thread.Sleep(10);
                canv.Display();
            }

            canv.Display();

            System.Console.ReadKey(true);
            Cosmos.System.Power.Reboot();
        }

        protected override void BeforeRun()
        {
            try
            {
                try { VFSManager.RegisterVFS(fs); } catch { }
                
                canv = new VBECanvas();

                bool throwTestError = false;
                if (throwTestError) { throw new Exception("This is a test exception"); }

                defFont = PCScreenFont.Default;
                MouseManager.ScreenWidth = canv.Mode.Width;
                MouseManager.ScreenHeight = canv.Mode.Height;
                MouseManager.X = MouseManager.ScreenWidth / 2;
                MouseManager.Y = MouseManager.ScreenHeight / 2;

                bg = new Bitmap(bgBytes);
                cursor = new Bitmap(curBytes);
                logo = new Bitmap(logoBytes);

                mainButton = new Button("Zenith", 0, 0, mainCol, defFont,7,logo);


                applications.Add(new Application(() => new Calc(), "Calculator", new Calc().logo));
                applications.Add(new Application(() => new CalcLegacy(), "Calculator (Legacy)",new CalcLegacy().logo));
                applications.Add(new Application(() => new Terminal(), "Terminal", new Terminal().logo));
                applications.Add(new Application(() => new TestWindow(), "Test Window",new TestWindow().logo));
                applications.Add(new Application(() => new UITest(), "Input Field Test", new UITest().logo));
                applications.Add(new Application(() => new About(), "About Zenith...", new About().logo));
                applications.Add(new Application(() => new Windows.Power(), "Power...",new Windows.Power().logo));
            
           
                for (int i = 0; i < applications.Count; i++)
                {
                    applicationsButtons.Add(new Button(applications[i].name, 30, 40 + i * 50, mainCol, defFont, 10, applications[i].logo,240));
                }

                try
                {
                    var mixer = new AudioMixer();
                    var audioStream = MemoryAudioStream.FromWave(sampleAudioBytes);
                    var driver = AC97.Initialize(bufferSize: 4096);
                }
                catch(Exception ex)
                {
                    ShowMessage(ex.Message, "Audio Driver Initialization Error",MsgType.Error);
                }
            }
            catch (Exception e)
            {
               HandleFatalError(e);
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
            
            canv.DrawImageAlpha(cursor, xPos, yPos);
        }
        
        protected override void Run()
        {
            try { 
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
            catch (Exception e)
            {
              HandleFatalError(e);
            }
        }
    }
}
