using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Transactions;
class Program
{
    public class CommandClass
    {
        public int Id;
        public string Name;
        public bool ArgumentAble;
        public bool TagAble;
        public List<string> TagsId = new();
        public Action<Instructions> Command;

        public CommandClass(int Id, string Name, bool ArgumentAble, bool TagAble, List<string> TagsId, Action<Instructions> cmd)
        {
            this.Id = Id;
            this.Name = Name;
            this.ArgumentAble = ArgumentAble;
            this.TagAble = TagAble;
            this.TagsId = TagsId;
            this.Command = cmd;
        }
    }

    static List<CommandClass> Commandslist = 
    [
        new CommandClass(1,"show", true, true, ["-h", "-f", "-a"], ShowFolders),
        new CommandClass(2,"help", true, false, [], Help),
        new CommandClass(3,"pwd", false, false, [], CurrentDirectory),
        new CommandClass(4,"create", true, true, ["-h", "-f"], MakeDirectory)
    ];

    public class Instructions
    {
        // Current Id
        static int CurrentId = 0;
        // Constructor for Instructions
        public Instructions()
        {
            _Id = ++CurrentId;
        }

        private int _Id;

        public int Id
        {
            get{ return _Id;}
        }
        
        // Is a valid command or not
        private int _IsCommand;

        // get and set command
        public int IsCommand
        {
            get
            { return _IsCommand;}
            set
            {
                if (value == 0 || value == 1)
                {
                    _IsCommand = value;
                }
            }
        }

        // Command ID (-1) if not a command
        private int _CommandID;

        // get and set command
        public int CommandID
        {
            get
            { return _CommandID;}
            set
            {
                if (value >= -1)
                {
                    _CommandID = value;
                }
            }
        }

        // Is Tag able or not
        private int _TagAble;

        // get and set command
        public int TagAble
        {
            get
            { return _TagAble;}
            set
            {
                if (value == 0 || value == 1)
                {
                    _TagAble = value;
                }
            }
        }
        // Is Arguement able or not
        private int _ArguementAble;

        // get and set command
        public int ArguementAble
        {
            get
            { return _ArguementAble;}
            set
            {
                if (value == 0 || value == 1)
                {
                    _ArguementAble = value;
                }
            }
        }
        // Counts the number of Tag
        private int _TagCount;

        // get and set command
        public int TagCount
        {
            get
            { return _TagCount;}
            set
            {
                if (value >= 0)
                {
                    _TagCount = value;
                }
            }
        }
        // Counts the number of Arguments
        private int _ArgumentsCount;

        // get and set command
        public int ArgumentsCount
        {
            get
            { return _ArgumentsCount;}
            set
            {
                if (value >= 0)
                {
                    _ArgumentsCount = value;
                }
            }
        }
        // List of the Tags of int data type
        private List<string> _TagsID = new();

        // get and set command
        public List<string> TagsID
        {
            get
            { return _TagsID;}
            set
            {
                _TagsID = value;
            }
        }
        // List of the Agruments of string data type
        private List<string> _Arguments = new();

        // get and set command
        public List<string> Arguments
        {
            get
            { return _Arguments;}
            set
            {
                _Arguments = value;
            }
        }

        public int ErrCode = 0;

        public string ErrMessage = "";

        public CommandClass? CommandObject;
    }

    static void ShowFolders(Instructions I)
    {
        string dir = "";
        int ToggleHiddenDirectories = 0;
        int ToggleFiles = 0;
        int ToggleFoldersandFiles = 0;

        // Tags toggler and checker
        foreach (string tag in I.TagsID)
        {
            switch (tag)
            {
                case "-h":
                    ToggleHiddenDirectories = 1;
                    break;

                case "-f":
                    ToggleFiles = 1;
                    break;

                case "-a":
                    ToggleFoldersandFiles = 1; // Also shows the files and folders in the directory 
                    break;
                
                default:
                    I.ErrMessage = "Invalid Tags";
                    I.ErrCode = 1;
                    return;
            }
        }

        // Arguments Count checker
        if (I.ArgumentsCount > 1)
        {
            I.ErrMessage = "To many Arguments";
            I.ErrCode = 1;
            return;
        }
        else
        {
            if (I.ArguementAble == 1 && I.ArgumentsCount == 1)
            {
                dir = I.Arguments[0];
            }
            else
            {
                dir = Directory.GetCurrentDirectory();
            }          
        }
        try {
            string[] folders;
            if (ToggleFiles == 0)
            {
                folders = Directory.GetDirectories(dir);
            }
            else
            {
                folders = Directory.GetFiles(dir);
            }   

            if (folders.Length == 0 || folders == null)
            {
                if (ToggleFiles == 0)
                {
                    Console.WriteLine("No folders");
                }
                else if (ToggleFoldersandFiles == 1)
                {
                    Console.WriteLine("No files or folders");
                }
                else
                {
                    Console.WriteLine("No files");
                }
            }            

            if (ToggleFoldersandFiles == 1)
            {
                string[] files = Directory.GetFiles(dir);
                folders = folders.Concat(files).ToArray();
            }

            foreach (string folder in folders)
            {
                FileAttributes attributes = File.GetAttributes(folder);
                int HiddenFolder = ((attributes & FileAttributes.Hidden) != 0) ? 1 : 0;

                if (HiddenFolder == 0)
                {
                    if((attributes & FileAttributes.Directory) != 0)
                    {
                        Console.WriteLine(".\\" + Path.GetFileName(folder));
                    }
                    else
                    {
                        Console.WriteLine(Path.GetFileName(folder));
                    }
                }
                else if (HiddenFolder == 1 && ToggleHiddenDirectories == 0)
                {
                    continue;
                }
                else if (HiddenFolder == 1 && ToggleHiddenDirectories == 1)
                {
                    if ((attributes & FileAttributes.Directory) != 0)
                    {
                        Console.WriteLine(".\\" + Path.GetFileName(folder));
                    }
                    else
                    {
                        Console.WriteLine(Path.GetFileName(folder));
                    }
                }
            }
        }
        catch (DirectoryNotFoundException)
        {
            I.ErrMessage = "Directory not found";
            I.ErrCode = 1;
        }
        catch (UnauthorizedAccessException)
        {
            I.ErrMessage = "Unauthorized access to file or directory";
            I.ErrCode = 1;
        }
        catch (IOException)
        {
            I.ErrMessage = "I/O error occurred";
            I.ErrCode = 1;
        }
    }

    static void Help(Instructions I)
    {
        string help_cmd = I.ErrMessage;
        string Errmessage = "";
        switch (help_cmd)
        {
            case "show":
                Errmessage = "Helping with the show!";  
                break;
            case "Bettle":
                Errmessage = "Helping with the Bettle!";
                break;    
            case "Not a Flagable":
                Errmessage = "Not a Tagable Command";
                break; 
            case "Not a Arguementable":
                Errmessage = "Not an Arguementable Command";
                break; 
            case "Not a Tag":
                Errmessage = "Not a tag supposed";
                break;    
            case "Use Default":
                Errmessage = "Using Defaults";
                break;          
            default:
                Errmessage = "";
                break;
        }
        Console.WriteLine(Errmessage);
    }

    static void CurrentDirectory(Instructions I)
    {
        // Checks whether the Tags are present any how
        if (I.TagAble == 1 || I.TagCount != 0 || I.TagsID.Count != 0)
        {
            I.ErrMessage = "Not a Flagable";
            I.ErrCode = 1;
            return;
        }
        // Checks whether any arguemtent is present or not
        else if (I.ArguementAble == 1 || I.ArgumentsCount != 0 || I.Arguments.Count != 0)
        {
            I.ErrMessage = "Not a Arguementable";
            I.ErrCode = 1;
            return;
        }
        else
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
        }    
    }

    static void MakeDirectory(Instructions I)
    {
        int ToggleHiddenDirectory = 0;
        int ToggleFile = 0;

        // Tags toggler and checker
        foreach (string tag in I.TagsID)
        {
            switch (tag)
            {
                case "-h":
                    ToggleHiddenDirectory = 1;
                    break;

                case "-f":
                    ToggleFile = 1;
                    break;
                
                default:
                    I.ErrMessage = "Invalid Tags";
                    I.ErrCode = 1;
                    return;
            }
        }

        for (int i = 0; i < I.Arguments.Count; i++)
        {
            try {
                string file;

                // If the passed argument is with a token & (for creating a file)
                if (I.Arguments[i].StartsWith("&"))
                {
                    // Combines current directory with file name and pass it on File Create
                    if (ToggleFile == 0)
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i].Substring(1));
                        File.Create(file).Dispose();
                        Console.WriteLine(I.Arguments[i].Substring(1) + " Created!");
                    }
                    else
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i].Substring(1));
                        Console.WriteLine(Directory.CreateDirectory(file) + " created!");
                    }
                }
                else
                {
                    if (ToggleFile == 0)
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i]);
                        Console.WriteLine(Directory.CreateDirectory(file) + " created!");
                    }
                    else
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i]);
                        File.Create(file).Dispose();
                        Console.WriteLine(I.Arguments[i] + " Created!");
                    }
                }

                if (ToggleHiddenDirectory == 1)
                {
                    File.SetAttributes(file, FileAttributes.Hidden);
                }
                
            }
            catch (UnauthorizedAccessException)
            {
                I.ErrMessage = "Unauthorized access to file or directory";
                I.ErrCode = 1;
            }
            catch (IOException)
            {
                I.ErrMessage = "I/O error occurred";
                I.ErrCode = 1;
            }
        }
    }

    static void TakeCommands()
    {
        string? command;
        do
        {
            Console.Write("Bettle > ");
            command = Console.ReadLine()?.Trim();
            if (command == "exit" || command == "quit" || command == "kill")
                break;
            if (command != null)
            {
                TakeActions(ValidateCommands(command));
            }
            else
            {
                continue;
            }
        }
        while (true);
        Console.WriteLine("Bettle Closing!");
    }

    static Instructions ValidateCommands(string command)
    {
        Instructions I = new();

        bool IsReplaced = false;
        int ReplacedCount = 0;
        int[] replaced = new int[command.Length];

        // Converts "     Show   -h    -g    dir   " into "show -h -g dir"
        string[] _Parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        command = string.Join(" ", _Parts);

        if (command.Contains("\""))
        {
            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == '"')
                {
                    for (int j = i + 1; j < command.Length; j++)
                    {
                        if (command[j] == ' ')
                        {
                            command = command.Remove(j, 1).Insert(j, "_");
                            IsReplaced = true;
                            replaced[ReplacedCount] = j;
                            ReplacedCount++;                            
                        }
                        else if (command[j] == '"')
                        {
                            break;
                        }               
                    }
                }
            }
        }
         
        int[] ReplacedWord = new int[command.Length];
        int[] ReplacedLetter = new int[command.Length];

        for (int j = 0; j < ReplacedCount; j++)
        {
            for (int i = 0; i < replaced[j]; i++)
            {
                if (command[i] == ' ')
                {
                    ReplacedWord[j]++;
                    ReplacedLetter[j] = 0;
                }
                else
                {
                    ReplacedLetter[j]++;
                }
            }
        }        

        // divides the command into sections 
        string[] Parts = command.Split(" ");

        if (IsReplaced)
        {
            for (int j = 0; j < ReplacedCount; j++)
            {
                Parts[ReplacedWord[j]] = Parts[ReplacedWord[j]].Remove(ReplacedLetter[j], 1).Insert(ReplacedLetter[j], " ");
            }
        }

        for (int i = 0; i < Parts.Length; i++)
        {
            if (Parts[i].StartsWith('"') || Parts[i].EndsWith('"'))
            {
                Parts[i] = Parts[i].Substring(1, Parts[i].Length - 2);
                Parts[i] = Parts[i].Remove(Parts[i].Length - 1, 0);
            }
        }

        // Checking whether the tags or arguemnt is present or not
        int ArgumentsCount = 0;
        int TagCount = 0; 
        int IsCommand = 0; // 1 for Command and 0 for not a command
        int CommandID = -1;
        CommandClass? CurrentCommand = null; // A null accepting Current Command of data type CommandClass

        // Checks for Empty Commands
        if (Parts[0] == "")
        {
            IsCommand = 0; // Not a command
        }
        else // condition if the command is not empty or just spaces
        {
            // checking whether the command is present in the commands list or not
            CurrentCommand = Commandslist.FirstOrDefault(c => c.Name == Parts[0].ToLower());

            // checking whether the command is actual command or not
            if (CurrentCommand != null)
            {
                CommandID = CurrentCommand.Id;
            }
            else if (CurrentCommand == null)
            {
                I.ErrMessage = "Not a Command";
                I.ErrCode = 1;
            }
            IsCommand = (CommandID == -1) ? 0 : 1;
        }

        if (CurrentCommand != null){    
            // Check whether the IsCommand is on or Parts.Length is greater than 1 so to access them.
            if (IsCommand == 1 && Parts.Length > 1){ 

                // Checks for the first Parts whether it is a tag or not
                if(Parts[IsCommand].StartsWith("-"))
                {
                    I.TagsID.Add(Parts[IsCommand]);
                    TagCount++;
                } 

                // Checking the count of Tags
                for (int i = IsCommand; i < Parts.Length; i++)
                {
                    // Checks whether the consecutive`s one are having the tag
                    if (Parts[i-1].StartsWith("-") && Parts[i].StartsWith("-"))
                    {
                        I.TagsID.Add(Parts[i]);
                        TagCount++;
                    }
                }

                // Checks Validity of Command`s TagAble
                if (TagCount > 0 && !(CurrentCommand.TagAble))
                {
                    I.ErrMessage = "Not a Flagable";
                    I.ErrCode = 1;
                }

                // Checking the count of arguments which are instered
                for (int i = IsCommand + TagCount; i < Parts.Length; i++) 
                {
                    if (!(Parts[i].StartsWith("-")))
                    {
                        I.Arguments.Add(Parts[i]);
                        ArgumentsCount++;
                    }
                    else
                    {
                        I.Arguments.Clear();
                        I.ArguementAble = 0;
                        I.ArgumentsCount = 0;
                        I.ErrMessage = "Invalid Arguement";
                        I.ErrCode = 1;
                        break;
                    }
                }

                // Checks Validity of Command`s ArgumentAble
                if (ArgumentsCount > 0 && !(CurrentCommand.ArgumentAble))
                {
                    I.ErrMessage = "Not a Arguementable";
                    I.ErrCode = 1;
                }
            }
        }

        // Filling the values of object
        I.IsCommand = IsCommand;
        I.CommandID = CommandID;
        I.TagAble = (TagCount != 0) ? 1 : 0;
        I.ArguementAble = (ArgumentsCount != 0) ? 1 : 0;
        I.TagCount = TagCount;
        I.ArgumentsCount = ArgumentsCount;
        I.CommandObject = CurrentCommand;

        return I;
    }

    static void TakeActions(Instructions I)
    {
        if (I.CommandObject == null)
        {
            return;
        }

        if (I.ErrCode == 1)
        {
            Help(I);
            return;
        }

        I.CommandObject.Command(I); 
    }

    static void Main()
    {
        TakeCommands();
    }

}
