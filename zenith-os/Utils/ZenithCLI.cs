using Cosmos.Core;
using Cosmos.System.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using zenithos.Windows;
using Color = System.Drawing.Color;
namespace zenithos.Utils
{
    public class CLICommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Aliases { get; private set; }

        public CLICommand(string name,string description, string[] aliases)
        {
            Name = name;
            Description = description;
            Aliases = aliases;
        }

        public virtual void Execute(List<string> args,Terminal instance) { }
    }

    public class CLIEcho : CLICommand
    {
        public CLIEcho() : base("Echo", "Echoes given string", new string[] { "echo","out"} ) {}
         
        public override void Execute(List<string> args, Terminal instance)
        {
            instance.curcol = Color.White;
            for(int i =1;i<args.Count;i++)
            {
                instance.print_str(args[i]+" ");
            }
            instance.print_str("\n");
        }
    }

    public class CLIClearScreen : CLICommand
    {
        public CLIClearScreen() : base("ClearScreen","Clears the screen", new string[] { "cls", "clear", "clr" }) { }

        public override void Execute(List<string> args,Terminal instance)
        {
            instance.print_clear();
        }
    }

    public class CLIInfo : CLICommand
    {
        public CLIInfo() : base("Info", "Gets System Info", new string[] { "sysinfo", "sysfetch", "info" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            //Following Code from z-izz/fetchOS
            // Thanks!
            string vmname = "Environment isn't virtualized";
            if (Cosmos.System.VMTools.IsVMWare)
            {
                vmname = "VMWare";
            }
            else if (Cosmos.System.VMTools.IsQEMU)
            {
                vmname = "QEMU or KVM";
            }
            else if (Cosmos.System.VMTools.IsVirtualBox)
            {
                vmname = "VirtualBox";
            }

            if (CPU.GetCPUVendorName().Contains("Intel")) // if intel chip
            {
                instance.curcol = Color.Aqua;
                instance.print_str($"88                              88 | ZENITH OS\n");
                instance.print_str($"''              ,d              88 | ---------\n");
                instance.print_str($"                88              88 | CPU : {Cosmos.Core.CPU.GetCPUBrandString()}\n");
                instance.print_str($"88 8b,dPPYba, MM88MMM ,adPPYba, 88 | RAM : {Cosmos.Core.CPU.GetAmountOfRAM()} MB\n");
                instance.print_str($"88 88P'   `'8a  88   a8P_____88 88 | VM : {vmname}\n");
                instance.print_str($"88 88       88  88   8PP''''''' 88 |\n");
                instance.print_str($"88 88       88  88,  '8b,   ,aa 88 |\n");
                instance.print_str($"88 88       88  'Y888 `'Ybbd8'' 88 |\n");
            }
            else if (CPU.GetCPUVendorName().Contains("AMD")) // if amd chip
            {
                instance.curcol = Color.Red;
                instance.print_str($"              *@@@@@@@@@@@@@@@     | ZENITH OS\n");
                instance.print_str($"                 @@@@@@@@@@@@@     | ---------\n");
                instance.print_str($"                @%       @@@@@     | CPU : {Cosmos.Core.CPU.GetCPUBrandString()}\n");
                instance.print_str($"              @@@%       @@@@@     | RAM : {Cosmos.Core.CPU.GetAmountOfRAM()} MB\n");
                instance.print_str($"             @@@@&       @@@@@     | VM : {vmname}\n");
                instance.print_str($"             @@@@@@@@@     @@@     | \n");
                instance.print_str($"             #######               | \n");
                instance.print_str($"                                   | \n");
                instance.print_str($"            @@     @\\ /@  @@@@*    | \n");
                instance.print_str($"           @..@    @ @ @  @.   @   | \n");
                instance.print_str($"          @    @   @   @  @@@@*    | \n");
            }
            instance.curcol = Color.White;
        }
    }

    public class CLIDir : CLICommand
    {
        public CLIDir() : base("Dir", "Outputs Current Directory Listing", new string[] { "dir", "ls" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            instance.print_str($" --- Directory Listing of {instance.pwd} ---\n");
            string [] dirs = Directory.GetDirectories(instance.pwd);
            instance.curcol = Color.Yellow;
            foreach(string dir in dirs)
            {
                instance.print_str($" - DIR   {dir}\n");
            }
            instance.curcol = Color.White;
            string[] files = Directory.GetFiles(instance.pwd);
            foreach (string file in files)
            {
                instance.print_str($" - FILE  {file}\n");
            }
            instance.curcol = Color.White;
        }
    }

    public class CLICD : CLICommand
    {
        public CLICD() : base("CD", "Change Directory", new string[] { "cd", "chdir" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            if(args.Count != 2)
            {
                instance.print_str("Usage: cd [dirpath]");
                return;
            }

            if (args[1] == "..")
            {
                instance.pwd = Directory.GetParent(instance.pwd).FullName;
            }
            else if (args[1] == ".")
            {
                return;
            }
            else
            {
                string oldpwd = instance.pwd;
                if(Path.IsPathRooted(args[1]))
                {
                    instance.pwd = args[1];
                }
                else
                {
                    instance.pwd = Path.Join(instance.pwd, args[1]);
                }
                if(!Directory.Exists(instance.pwd))
                {
                    instance.curcol = Color.Red;
                    instance.print_str($"[ERR] No such path {instance.pwd}\n");
                    instance.curcol = Color.White;
                    instance.pwd = oldpwd;
                }
            }
        }

    }

    public class CLICat : CLICommand
    {
        public CLICat() : base("Cat", "Read file", new string[] { "cat", "read" }) { }
        public override void Execute(List<string> args, Terminal instance)
        {
            if (args.Count != 2)
            {
                instance.print_str("Usage: cat [dirpath]");
                return;
            }
            string fpath = "";

            if (Path.IsPathRooted(args[1]))
            {
                fpath = args[1];
            }
            else
            {
                fpath = Path.Join(instance.pwd, args[1]);
            }

            if (!File.Exists(fpath))
            {
                instance.curcol = Color.Red;
                instance.print_str($"[ERR] No such file {fpath}\n");
                instance.curcol = Color.White;
            }

            string contents = File.ReadAllText(fpath);

            instance.print_str(contents);
        }
    }




    public class ZenithCLI
    {


        public static CLICommand[] Commands = {
            new CLIClearScreen(),
            new CLIInfo(),
            new CLIEcho(),
            new CLIDir(),
            new CLICD(),
            new CLICat(),
        };
        public static void ParseCommand(string command,Terminal instance)
        {
            List<string> args = command.Split(' ').ToList();

            foreach(string arg in args)
            {
                if(string.IsNullOrEmpty(arg))
                {
                    args.Remove(arg);
                }
            }

            foreach(CLICommand cmd in Commands)
            {
                foreach(string alias in cmd.Aliases)
                {
                    if (alias.ToLower() == args[0].ToLower())
                    {
                        cmd.Execute(args, instance);
                        //instance.print_str($"Found Command {cmd.Name} from alias {alias}");
                        return;
                    }
                }
            }
            instance.curcol = Color.Red;
            instance.print_str($"[ERR] No such command {args[0]}\n");
            instance.curcol = Color.White;
        }
    }
}
