using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Transactions;
class Program
{
    static string[] Commands = {"help", "show"};

    public class CommandClass
    {
        int Id;
        public string Name;
        bool ArgumentAble;
        bool TagAble;
        public List<string> TagsId = new();
        public Action<Instructions> Command;

        public Commands(int Id, string Name, bool ArgumentAble, bool TagAble, List<string> TagsId, Action<Instructions> cmd)
        {
            this.Id = Id;
            this.Name = Name;
            this.ArgumentAble = ArgumentAble;
            this.TagAble = TagAble;
            this.TagsId = TagsId;
            this.Command = cmd;
        }
    }

    List<CommandClass> Commandslist = 
    [
        new CommandClass(1,"show", true, true, ["-h", "-f", "-hf"], ShowFolders),
        new CommandClass(2,"help", true, false, [], Help)
    ];

    static string[,] Tags =
    {
        {"-h", "Show hidden Directories"},
        {"-f", "Show Files"},
        {"-hf", "Show Files and Directories"}
    };

    public class Instructions
    {
        // Current Id
        int CurrentId = 0;
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
        private List<string> _Arguements = new();

        // get and set command
        public List<string> Arguements
        {
            get
            { return _Arguements;}
            set
            {
                _Arguements = value;
            }
        }
    }

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
                message = "Not a Tagable Command";
                break; 
            case "Not a Arguementable":
                message = "Not an Arguementable Command";
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
        string command;
        do
        {
            Console.Write("Bettle > ");
            command = Console.ReadLine().ToLower();
            TakeActions(ValidateCommands(command));
        }
        while (!(command == "exit" || command == "quit" || command == "kill"));
        Console.WriteLine("Bettle Closing!");
    }

    static Instructions ValidateCommands(string command)
    {
        // Converts "     Show   -h    -g    dir   " into "show -h -g dir"
        string[] _Parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        command = string.Join(" ", _Parts);

        // divides the command into sections 
        string[] Parts = command.Split(" ");
        foreach (string item in Parts)
        {
            Console.WriteLine(item);
        }

        // Checking whether the tags or arguemnt is present or not
        int ArgumentsCount = 0;
        int TagCount = 0; 
        int IsCommand = 0; // 1 for Command and 0 for not a command
        int CommandID = -1;
        CommandClass? CurrentCommand; // A null accepting Current Command of data type CommandClass

        // Checks for Empty Commands
        if (Parts[0] == "")
        {
            IsCommand = 0; // Not a command
        }
        else // condition if the command is not empty or just spaces
        {
            // checking whether the command is present in the commands list or not
            CurrentCommand = Commandslist.FirstOrDefault(c => c.Name == Parts[0]);

            // checking whether the command is actual command or not
            if (CurrentCommand != null)
            {
                CommandID = CurrentCommand.Id;
            }
            IsCommand = (CommandID == -1) ? 0 : 1;
        }

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
                Help("Not a Flagable");
            }

            // Checking the count of arguements which are instered
            for (int i = IsCommand + TagCount; i < Parts.Length; i++) 
            {
                if (!(Parts[i].StartsWith("-")))
                {
                    I.Arguements.Add(Parts[i]);
                    ArgumentsCount++;
                }
                else
                {
                    I.Arguements.Clear();
                    Help("Invalid Arguement"); 
                    break;
                }
            }

            // Checks Validity of Command`s ArgumentAble
            if (ArgumentsCount > 0 && !(CurrentCommand.ArgumentAble))
            {
                Help("Not a Arguementable");
            }
        }

        Instructions I = new();

        // Filling the values of object
        I.IsCommand = IsCommand;
        I.CommandID = CommandID;
        I.TagAble = (TagCount != 0) ? 1 : 0;
        I.ArguementAble = (ArgumentsCount != 0) ? 1 : 0;
        I.TagCount = TagCount;
        I.ArgumentsCount = ArgumentsCount;

        return I;
    }

    static void TakeActions(Instructions I)
    {
        Console.WriteLine("ID: " + I.Id);
        Console.WriteLine("Is a Command: " + I.IsCommand);
        Console.WriteLine("Command ID: " + I.CommandID);
        Console.WriteLine("Is Tag-able: " + I.TagAble);
        Console.WriteLine("Tags Count: " + I.TagCount);
        Console.WriteLine("Is Arguement-able: " + I.ArguementAble);
        Console.WriteLine("Arguements Count: " + I.ArgumentsCount);

        // Prints every element of the Tags
        if (I.TagAble == 1){
            Console.Write("Tags: ");
            for (int i = 0; i < I.TagsID.Count; i++)
            {
                string item = I.TagsID[i];
                Console.Write(item);
                if (i != I.TagsID.Count - 1)
                {
                    Console.Write(", ");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }
        
        // Prints every element of the Arguements
        if (I.ArguementAble == 1){
            Console.Write("Arguements: ");
            for (int i = 0; i < I.Arguements.Count; i++)
            {
                string item = I.Arguements[i];
                Console.Write(item);
                if (i != I.Arguements.Count - 1)
                {
                    Console.Write(", ");
                }
                else
                {
                    Console.WriteLine();
                }
            }
        }   
    }

    static void Main()
    {
        TakeCommands();
    }

}
