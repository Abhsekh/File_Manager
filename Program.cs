class Program
{
    // Class for the Commands
    public class CommandClass
    {
        // Command ID
        public int Id;
        // Command Name
        public string Name;
        // Is the command Argument able or not
        public bool ArgumentAble;
        // Is the command Tag able or not
        public bool TagAble;
        // List of the Tags of string data type
        public List<string> TagsId = new();
        // Function of the command
        public Action<Instructions> Command;

        // Constructor for the CommandClass
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

    // List of Commands
    static List<CommandClass> Commandslist = 
    [
//      new CommandClass(S.N., Command     , Arguments, Tags   , Lists of tags allowed , Function)          ,
        new CommandClass(1   , "show"      , true     , true   , ["-h", "-f", "-a"]    , ShowFolders)       ,
        new CommandClass(2   , "help"      , true     , false  , []                    , Help)              ,
        new CommandClass(3   , "cd"        , true     , false  , []                    , CurrentDirectory)  ,
        new CommandClass(4   , "create"    , true     , true   , ["-h", "-f"]          , MakeDirectory)     ,
        new CommandClass(5   , "murder"    , true     , true   , ["-f"]                , RemoveDirectory)   ,
        new CommandClass(6   , "stage"     , true     , true   , ["-s", "-c", "-r"]    , Stage)             ,
        new CommandClass(7   , "move"      , true     , true   , ["-r"]                , Move)
    ];

    // Class for the Instructions
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
                string[]? files = Directory.GetFiles(dir);

                // If folders is null, use an empty array instead, use ?? for nullable coalescing operator
                folders = (folders ?? Array.Empty<string>()).Concat(files).ToArray();
            }

            // If folders is null, use an empty array instead, use ?? for nullable coalescing operator
            foreach (string? folder in (folders ?? Array.Empty<string>()))
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
            else if (I.ArguementAble == 1 && I.ArgumentsCount > 0 && I.Arguments.Count > 0 && I.ArgumentsCount < 2 && I.Arguments.Count < 2)
        {
            try {
                Directory.SetCurrentDirectory(I.Arguments[0]);
                Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory not found");
                Console.WriteLine("Would you like to create the directory? (y/n)");
                string? response = Console.ReadLine()?.Trim().ToLower();
                if (response == "y" || response == "yes")
                {
                    MakeDirectory(I);
                }
                else
                {
                    return;
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

        // This is from where the commands starts
        for (int i = 0; i < I.Arguments.Count; i++)
        {
            try 
            {
                string file;

                // If the passed argument is with a token & (for creating a file)
                if (I.Arguments[i].StartsWith("&"))
                {
                    // Combines current directory with file name and pass it on File Create
                    if (ToggleFile == 0)
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i].Substring(1));
                        if (File.Exists(file))
                        {
                            Console.WriteLine(I.Arguments[i].Substring(1) + " Already Exists!");
                        }
                        else
                        {
                            File.Create(file).Dispose();
                            Console.WriteLine(I.Arguments[i].Substring(1) + " Created!");
                        }
                    }
                    else
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i].Substring(1));
                        if (!Directory.Exists(file))
                        {
                            Console.WriteLine(Directory.CreateDirectory(file) + " created!");
                        }
                        else
                        {
                            Console.WriteLine(file + " Already Exists!");
                        }
                    }
                }
                else
                {
                    if (ToggleFile == 0)
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i]);
                        if (!Directory.Exists(file))
                        {
                            Console.WriteLine(Directory.CreateDirectory(file) + " created!");
                        }
                        else
                        {
                            Console.WriteLine(file + " Already Exists!");
                        }
                    }
                    else
                    {
                        file = Path.Combine(Directory.GetCurrentDirectory(), I.Arguments[i]);
                        if (File.Exists(file))
                        {
                            Console.WriteLine(I.Arguments[i] + " Already exists!");
                        }
                        else
                        {
                            File.Create(file).Dispose();
                            Console.WriteLine(I.Arguments[i] + " Created!");
                        }
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

    static void RemoveDirectory(Instructions I)
    {
        int ToggleFile = 0;

        // Tags toggler and checker
        foreach (string tag in I.TagsID)
        {
            switch (tag)
            {
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
            try
            {
                // If Wild - Card Enters
                if (I.Arguments[i].StartsWith("&"))
                {
                    // Delete the folder or file
                    if (ToggleFile == 0)
                    {
                        if (File.Exists(I.Arguments[i].Substring(1)))
                        {
                            // Delete the files
                            File.Delete(I.Arguments[i].Substring(1));
                            Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i].Substring(1) + " is deleted successfully!");
                        }
                        else
                        {
                            I.ErrMessage = "File does not exists";
                            I.ErrCode = 1;
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(I.Arguments[i].Substring(1)))
                        {
                            I.ErrMessage = "Folder does not exists";
                            I.ErrCode = 1;
                        }
                        else
                        {
                            // Delete the folders
                            if (Directory.EnumerateFileSystemEntries(I.Arguments[i].Substring(1)).Any())
                            {
                                Console.Write("The folder \"" + Directory.GetCurrentDirectory() + "\\" +  I.Arguments[i].Substring(1) + "\" is not empty, do you wanna go on(y/n): ");
                                // If the user does not enters anything, it will be considered as No
                                if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                {
                                    Directory.Delete(I.Arguments[i].Substring(1), true);
                                    Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i].Substring(1) + " deleted with its subdirectories!");
                                }
                            }
                            else
                            {
                                Directory.Delete(I.Arguments[i].Substring(1));
                                Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i].Substring(1) + " deleted!");
                            }
                        }
                    }                   
                }
                else
                {
                    // Delete the folder or file
                    if (ToggleFile == 1)
                    {
                        // Delete the files
                        if (File.Exists(I.Arguments[i]))
                        {
                            File.Delete(I.Arguments[i]);
                            Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i] + " is deleted successfully!");
                        }
                        else
                        {
                            I.ErrMessage = "File does not exists";
                            I.ErrCode = 1;
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(I.Arguments[i]))
                        {
                            I.ErrMessage = "Folder does not exists";
                            I.ErrCode = 1;
                        }
                        else
                        {
                            // Delete the folders
                            if (Directory.EnumerateFileSystemEntries(I.Arguments[i]).Any())
                            {
                                Console.Write("The folder \"" + Directory.GetCurrentDirectory() + "\\" + I.Arguments[i] + "\" is not empty. Do you want to continue (Y/N)? ");
                                if ((Console.ReadLine()  ?? "N").ToUpper() == "Y")
                                {
                                    Directory.Delete(I.Arguments[i], true);
                                    Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i] + " deleted with its subdirectories!");
                                }
                            }
                            else
                            {
                                Directory.Delete(I.Arguments[i]);
                                Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i] + " deleted!");
                            }
                        }
                    }     
                }
            }
            catch (FileNotFoundException)
            {
                I.ErrMessage = "File does not exists";
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
    }

    static void Stage(Instructions I)
    {
        // Tags
        bool ToggleShow = false;
        bool ToggleClear = false;
        bool ToggleMake = false;
        bool ToggleRemove = false;
        bool ToggleDelete = false;

        // Tags toggler and checker
        foreach (string tag in I.TagsID)
        {
            switch (tag)
            {
                case "-s":
                    ToggleShow = true;
                    break;

                case "-c":
                    ToggleClear = true;
                    break;

                case "-m":
                    ToggleMake = true;
                    break;

                case "-r":
                    ToggleRemove = true;
                    break;

                case "-d":
                    ToggleDelete = true;
                    break;
                
                default:
                    I.ErrMessage = "Invalid Tags";
                    I.ErrCode = 1;
                    return;
            }
        }

        if (ToggleShow)
        {
            if (File.Exists("data.io"))
            {
                string[] paths = File.ReadLines("data.io").Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

                Console.WriteLine("Staged Files: " + paths.Length);

                if (paths.Length == 0)
                {
                    Console.WriteLine("No Staged Files");
                }
                else
                {
                    for (int i = 0; i < paths.Length; i++)
                    {
                        Console.WriteLine($"{i + 1}. {paths[i]}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Staged Files: 0");
                Console.WriteLine("No Staged Files");
                File.Create("data.io").Dispose();
            }
        }

        if (ToggleClear)
        {
            if (File.Exists("data.io"))
            {
                File.Delete("data.io");
                Console.WriteLine("Stage is Off!");
            }
            File.Create("data.io").Dispose();
        }

        if (ToggleRemove)
        {
            Console.WriteLine("No. of Arguements: " + I.ArgumentsCount);
            for (int i = 0; i < I.ArgumentsCount; i++)
            {
                int n;
                bool parsed = int.TryParse(I.Arguments[i], out n);

                if (parsed)
                {
                    string[] lines = File.ReadAllLines("data.io");

                    if (n >= 0 && n < lines.Length)
                    {
                        lines = lines.Where((line, index) => index != n).ToArray();

                        File.WriteAllLines("data.io", lines); // Use WriteAllLines to overwrite the file with the updated lines

                        Console.WriteLine(n + " has been thrown out of the stage!");
                    }
                }
                else
                {
                    I.ErrMessage = "Invalid Index";
                }
            }
        }

        if (I.ArgumentsCount > 0 && !ToggleRemove && !ToggleDelete && !ToggleMake)
        {
            for (int i = 0; i < I.ArgumentsCount; i++)
            {
                // Used to Stage whole Folder and its Sub directories
                if (I.Arguments[i].StartsWith("&"))
                {
                    // create the file if it does not exist
                    if (!File.Exists("data.io"))
                    {
                        File.Create("data.io").Dispose();
                    }
                    else
                    {
                        // Checks whether the folders exists or not
                        if (File.ReadLines("data.io").Contains(Path.GetFullPath(I.Arguments[i].Substring(1))))
                        {
                            Console.WriteLine(Path.GetFullPath(I.Arguments[i].Substring(1)) + " Already Staged!");
                            continue;
                        }
                    }

                    // Append the folders
                    File.AppendAllText("data.io", "\n" + Path.GetFullPath(I.Arguments[i].Substring(1))); // used to append File path
                    Console.WriteLine(Path.GetFullPath(I.Arguments[i].Substring(1)) + " has Staged!");

                    string[] folders = Directory.GetFileSystemEntries(Path.GetFullPath(I.Arguments[i].Substring(1)),"*",SearchOption.AllDirectories);
                    foreach (string folder in folders)
                    {
                        // Checks whether the folders exists or not
                        if (File.ReadLines("data.io").Contains(Path.GetFullPath(folder)))
                        {
                            Console.WriteLine(Path.GetFullPath(folder) + " Already Staged!");
                        }
                        File.AppendAllText("data.io", "\n" + Path.GetFullPath(folder)); // used to append File path
                        Console.WriteLine(Path.GetFullPath(folder) + " has Staged!");
                    }
                }
                else
                {
                    // create the file if it does not exist
                    if (!File.Exists("data.io"))
                    {
                        File.Create("data.io").Dispose();
                    }
                    else
                    {
                        // Checks whether the folders exists or not
                        if (File.ReadLines("data.io").Contains(Path.GetFullPath(I.Arguments[i])))
                        {
                            Console.WriteLine(Path.GetFullPath(I.Arguments[i]) + " Already Staged!");
                            continue;
                        }
                        File.AppendAllText("data.io", "\n" + Path.GetFullPath(I.Arguments[i])); // used to append File path
                        Console.WriteLine(Path.GetFullPath(I.Arguments[i]) + " has Staged!");
                    }
                }
            }
        }
    }

    static void Move(Instructions I)
    {
        bool ToggleCopy = false;

        // Tags toggler and checker
        foreach (string tag in I.TagsID)
        {
            switch (tag)
            {
                case "-c":
                    ToggleCopy = true;
                    break;
                
                default:
                    I.ErrMessage = "Invalid Tags";
                    I.ErrCode = 1;
                    return;
            }
        }

        // Check whether the number of arguments is even or not, because we need pairs of source and destination paths
        if (I.ArgumentsCount % 2 != 0)
        {
            I.ErrMessage = "Invalid number of arguments. Please provide source and destination paths.";
            I.ErrCode = 1;
            return;
        }

        for (int i = 0; i < I.ArgumentsCount; i += 2)
        {
            // Condition of renaming a file with the help of & token
            if (I.Arguments[i].StartsWith("&") && I.Arguments[i + 1].StartsWith("&"))
            {
                string sourcePath = Path.GetFullPath(I.Arguments[i].Substring(1));
                string destinationPath = Path.GetFullPath(I.Arguments[i + 1].Substring(1));

                try
                {
                    if (File.Exists(sourcePath))
                    {
                        File.Move(sourcePath, destinationPath);
                        Console.WriteLine($"File moved from {sourcePath} to {destinationPath}");
                    }
                    else
                    {
                        I.ErrMessage = "Source path does not exist.";
                        I.ErrCode = 1;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    I.ErrMessage = $"Error moving: {ex.Message}";
                    I.ErrCode = 1;
                    return;
                }
            }
            // Condition of moving a file from one directory to another directory with the help of & token
            else if (I.Arguments[i].StartsWith("&"))
            {
                string sourcePath = Path.GetFullPath(I.Arguments[i].Substring(1));
                string destinationPath = string.Concat(Path.GetFullPath(I.Arguments[i + 1]), string.Concat("\\", Path.GetFileName(sourcePath)));

                try
                {
                    if (File.Exists(sourcePath))
                    {
                        File.Move(sourcePath, destinationPath);
                        Console.WriteLine($"File moved from {sourcePath} to {destinationPath}");
                    }
                    else
                    {
                        I.ErrMessage = "Source path does not exist.";
                        I.ErrCode = 1;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    I.ErrMessage = $"Error moving: {ex.Message}";
                    I.ErrCode = 1;
                    return;
                }
            }
            // condition of moving a folder from one directory to another directory
            else
            {
                string sourcePath = Path.GetFullPath(I.Arguments[i]);
                string destinationPath = Path.GetFullPath(I.Arguments[i + 1]);

                try
                {
                    if (Directory.Exists(sourcePath))
                    {
                        destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
                        Directory.Move(sourcePath, destinationPath);
                        Console.WriteLine($"Directory moved from {sourcePath} to {destinationPath}");
                    }
                    else
                    {
                        I.ErrMessage = "Source path does not exist.";
                        I.ErrCode = 1;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    I.ErrMessage = $"Error moving: {ex.Message}";
                    I.ErrCode = 1;
                    return;
                }                
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