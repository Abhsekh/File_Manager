using System;
using System.ComponentModel.Design;
using System.IO;
class Program
{
    static void ShowFolders(string dir, int ToggleHiddenFolder = 0, int ToggleFiles = 0)
    {
        try {
            string[] folders = {""};
            folders = Directory.GetDirectories(dir);

            foreach (string folder in folders)
            {
                FileAttributes attributes = File.GetAttributes(folder);
                int HiddenFolder = ((attributes & FileAttributes.Hidden) != 0) ? 1 : 0;

                if (HiddenFolder == 0)
                {
                    Console.WriteLine(Path.GetFileName(folder));
                }
                else if (HiddenFolder == 1 && ToggleHiddenFolder == 0)
                {
                    continue;
                }
                else if (HiddenFolder == 1 && ToggleHiddenFolder == 1)
                {
                    Console.WriteLine(Path.GetFileName(folder));
                }
            }
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Directory does not exist.");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Access denied.");
        }
    }

    static void Help(string help_cmd)
    {
        if (help_cmd == "show")
        {
            Console.WriteLine("Helping with the show!");
        }
        else if (help_cmd == "Bettle")
        {
            Console.WriteLine("Helping with the Bettle!");
        }
    }

    static void ShowFolderCycle(string flag, string dir)
    {
        if (flag == "")
        {
            ShowFolders(dir);
        }
        else
        {
            switch (flag)
            {
                case "-h":
                    ShowFolders(dir, 1);
                    break;
                case "-f":
                    ShowFolders(dir);
                    break;
                case "-hf":
                    ShowFolders(dir, 1);
                    break;
                default:
                    Help("show");
                    break;
            }
        }
    }

    static void TakeCommands()
    {
        string command;
        do
        {
            Console.Write("Bettle > ");
            command = Console.ReadLine();
            TakeActions(command);
        }
        while (!(command == "exit" || command == "quit" || command == "kill"));
        Console.WriteLine("Bettle Closing!");
    }

    static void TakeActions(string command)
    {
        int Flagged = 0;
        string[] parts = command.Split(' ');
        if (command == "" || parts.Length < 1)
        {
            Help("Bettle");
        }
        else{
            if (parts[0] == "show")
            {
                if (parts.Length < 2)
                {
                    Help("show");
                }
                else
                {
                    if (parts[1].Replace(" ", "") == "")
                    {
                        Help("show");
                    }
                    else{
                        Flagged = (parts[1].Contains("-")) ? 1 : 0;
                        if (Flagged == 1)
                        {
                            if (parts.Length < 3)
                            {
                                Help("show");
                            }
                            else
                            {
                                if (parts[2].Replace(" ", "") == "")
                                {
                                    Help("show");
                                }
                                else{
                                    ShowFolderCycle(parts[1], parts[2]);
                                }
                            }
                        }
                        else
                        {
                            if (parts.Length > 2)
                            {
                                Help("Syntax Error");
                            }
                            ShowFolderCycle("", parts[1]);
                        }
                    }
                }
            }
            else if (parts[0] == "drive" || parts[0] == "drive" || parts[0] == "Drive" || parts[0] == "Drives")
            {
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    Console.WriteLine(drive.Name);
                }
            }
            else
            {
                Console.WriteLine("Control your horses! And slow down the pace.");
            }
        }
    }

    static void Main()
    {
        TakeCommands();
    }

}

