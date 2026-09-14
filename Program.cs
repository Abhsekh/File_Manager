using System;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
class Program
{
    static string[,] Commands =
    {
        {"help", "Single", "Non-Flagable", "Arguementable", "", ""},
        {"show", "Married", "Flagable", "Non-Arguementable", "0", ""},
        {"drive", "Single", "Non-Flagable", "Non-Arguementable", "0", ""},
        {"drives", "Single", "Non-Flagable", "Non-Arguementable", "0", ""}
    };

    static string[,] Tags =
    {
        {"-h", "Show hidden Directories"},
        {"-f", "Show Files"},
        {"-hf", "Show Files and Directories"}
    };

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
        string message = "";
        switch (help_cmd)
        {
            case "show":
                message = "Helping with the show!";  
                break;
            case "Bettle":
                message = "Helping with the Bettle!";
                break;    
            case "Not a Flagable":
                message = "Not a Taggable Command";
                break; 
            case "Not a Tag":
                message = "Not a tag supposed";
                break;    
            case "Use Default":
                message = "Using Defaults";
                break;          
            default:
                message = "";
                break;
        }
        Console.WriteLine(message);
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
        while (true)
        {
            Console.Write("Bettle > ");
            string command = Console.ReadLine();
            if (command == null)
            {
                break;
            }

            if (!ValidateCommands(command))
                continue;

            string[] parts = SplitCommand(command);
            if (parts.Length > 0 && IsExitCommand(parts[0]))
            {
                break;
            }
        }
        Console.WriteLine("Bettle Closing!");
    }

    static bool ValidateCommands(string command)
    {
        string[] parts = SplitCommand(command);
        if (parts.Length == 0)
        {
            return false;
        }

        string commandName = parts[0].ToLowerInvariant();
        int CommandId = -1;
        for (int i = 0; i < Commands.GetLength(0); i++)
        {
            if (Commands[i, 0] == commandName)
            {
                CommandId = i;
                break;
            }
        }

        if (CommandId == -1)
        {
            Help("Bettle");
            return false;
        }

        if (commandName == "show")
        {
            if (parts.Length < 2)
            {
                Help("show");
                return false;
            }

            if (parts[1].StartsWith("-"))
            {
                bool validTag = false;
                for (int i = 0; i < Tags.GetLength(0); i++)
                {
                    if (Tags[i, 0] == parts[1])
                    {
                        validTag = true;
                        break;
                    }
                }

                if (!validTag || parts.Length != 3)
                {
                    Help(!validTag ? "Not a Tag" : "Syntax Error");
                    return false;
                }
            }
            else if (parts.Length != 2)
            {
                Help("Syntax Error");
                return false;
            }
        }

        if (Commands[CommandId, 1] == "Single" && parts.Length != 1)
        {
            Help("Syntax Error");
            return false;
        }

        return true;
    }

    static string[] SplitCommand(string command)
    {
        List<string> parts = new List<string>();
        StringBuilder part = new StringBuilder();
        bool inQuotes = false;

        foreach (char character in command)
        {
            if (character == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (char.IsWhiteSpace(character) && !inQuotes)
            {
                if (part.Length > 0)
                {
                    parts.Add(part.ToString());
                    part.Clear();
                }
            }
            else
            {
                part.Append(character);
            }
        }

        if (inQuotes)
        {
            return Array.Empty<string>();
        }

        if (part.Length > 0)
        {
            parts.Add(part.ToString());
        }

        return parts.ToArray();
    }

    static bool IsExitCommand(string command)
    {
        return command == "exit" || command == "quit" || command == "kill";
    }

    static void TakeActions(string command)
    {
        string[] parts = SplitCommand(command);
        if (parts.Length == 0)
        {
            Help("Bettle");
        }
        else if (parts[0].ToLowerInvariant() == "help")
        {
            Help("Bettle");
        }
        else if (parts[0].ToLowerInvariant() == "show")
        {
            if (parts[1].StartsWith("-"))
            {
                ShowFolderCycle(parts[1], parts[2]);
            }
            else
            {
                ShowFolderCycle("", parts[1]);
            }
        }
        else if (parts[0].ToLowerInvariant() == "drive" || parts[0].ToLowerInvariant() == "drives")
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                Console.WriteLine(drive.Name);
            }
        }
    }

    static void Main()
    {
        TakeCommands();
    }

}

