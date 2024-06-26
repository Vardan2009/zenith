﻿using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Threading;
using zenithos.Utils;
using Color = System.Drawing.Color;

namespace zenithos.Windows
{
    public class Terminal : Window
    {
        public Color curcol = Color.White;
        int rown = 50,coln = 50;
        string[] content;
        private Color[][] colors;
        int at = 0;
        bool canwrite = false;
        public static string clilogo = "\n _____              _ __  __    ________    ____\n/__  /  ___  ____  (_) /_/ /_  / ____/ /   /  _/\n  / /  / _ \\/ __ \\/ / __/ __ \\/ /   / /    / /  \n / /__/  __/ / / / / /_/ / / / /___/ /____/ /   \n/____/\\___/_/ /_/_/\\__/_/ /_/\\____/_____/___/   \n                                                \n";

        public string pwd = @"0:\";

        void print_newline()
        {
            at++;

            if (at >= rown)
            {
                at = rown - 1;
                for (int i = 0; i < rown - 1; i++)
                {
                    content[i] = content[i + 1];
                    colors[i] = new Color[coln];
                    for (int j = 0; j < colors[i].Length; j++)
                    {
                        colors[i][j] = colors[i + 1][j];
                    }
                }
            }

            content[at] = "";
            for (int i = 0; i < coln; i++)
            {
                colors[at][i] = Color.Black;
            }
        }

        void print_char(char c)
        {
           if (content[at].Length >= coln)
           {
               print_newline();
           }

           if (c == '\n')
           {
               print_newline();
               return;
           }
                
           content[at] += c;
           colors[at][content[at].Length - 1] = curcol;
        }

        public void print_str(string str)
        {

            foreach(char c in str)
            {
                    print_char(c);
            }

        }

        public Terminal() : base(300, 300, 700, 300, "Terminal", Kernel.defFont, false)
        {
            rown = (int)(h / Kernel.defFont.Height);
            coln = (int)(w / Kernel.defFont.Width);
            content = new string[rown];
            colors = new Color[rown][];

            for (int i =0;i<rown;i++)
            {
                colors[i] = new Color[coln];
                content[i] = "";
            }
        }

        public void print_clear()
        {
            for (int i = 0; i < rown; i++)
            {
                colors[i] = new Color[coln];
                content[i] = "";
            }
            at = 0;
            curcol = Color.White;
        }

        void input_prefix()
        {
            curcol = Color.Yellow;
            print_str("zenith");
            curcol = Color.White;
            print_char('@');
            curcol = Color.Aqua;
            print_str(pwd);
            curcol = Color.White;
            print_str("$- ");
            curcol = Color.White;
        }

        void parse_input(string s)
        {
            ZenithCLI.ParseCommand(s, this);
            print_newline();
            input_prefix();
            canwrite = true;
        }

        public override void Start(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            curcol = Color.Aqua;
            print_str(clilogo);
            curcol = Color.White;
            print_str("Welcome to ZenithCLI, Command Line Interface for Zenith\n\n");
            canwrite = true;
            input_prefix();
        }
        string inpstr = "";
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);
            canv.DrawFilledRectangle(Color.Black, x+1, y + window_titlebarsize+1, w-1, h-1);
            for (int i = 0;i < content.Length;i++)
            {
                //canv.DrawString(content[i], font, Color.White, x+1, y + 1 + window_titlebarsize + i*Kernel.defFont.Height);
                for(int j = 0; j < content[i].Length;j++)
                {
                    canv.DrawChar(content[i][j], font, colors[i][j], x + 1 + (j * Kernel.defFont.Width), y + 1 + window_titlebarsize + i * Kernel.defFont.Height);
                }
            }
            
            
            if(canwrite && myIndex == Kernel.activeIndex)
            {
                canv.DrawChar('_', font, curcol, x + 1 + ((content[at].Length+inpstr.Length) * Kernel.defFont.Width), y + 1 + window_titlebarsize + at * Kernel.defFont.Height);
                canv.DrawString(inpstr, font, curcol, x + 1 + (content[at].Length) * Kernel.defFont.Width, y + 1 + window_titlebarsize + at * Kernel.defFont.Height);
                if (KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    if (key.Key == ConsoleKeyEx.Backspace)
                    {
                        if(inpstr.Length>0)
                            inpstr = inpstr.Remove(inpstr.Length - 1);
                    }
                    else if (key.Key == ConsoleKeyEx.Enter)
                    {
                        print_str(inpstr+"\n");
                        canwrite = false;
                        parse_input(inpstr);
                        inpstr = "";
                    }
                    else
                    {
                        if(inpstr.Length < coln-4)
                            inpstr += key.KeyChar;
                    }
                }
            }
        }
    }
}
