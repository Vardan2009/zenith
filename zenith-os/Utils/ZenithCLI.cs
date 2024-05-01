﻿using Cosmos.Core;
using System.Collections.Generic;
using System.Linq;
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
                instance.print_str($"          @    @   @   @  @@@@*    |\n");
            }
            instance.curcol = Color.White;
        }
    }


    public class ZenithCLI
    {
        public static CLICommand[] Commands = {
            new CLIClearScreen(),
            new CLIInfo(),
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
        }
    }
}
