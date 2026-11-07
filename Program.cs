using System.IO.Compression;

class Program
{
    // Command class
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
     // new CommandClass(S.N., Command     , Arguments, Tags   , Lists of tags allowed , Function)          ,
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
        private bool _TagAble;

        // get and set command
        public bool TagAble
        {
            get
            { return _TagAble;}
            set
            {
                if (value == false || value == true)
                {
                    _TagAble = value;
                }
            }
        }
        // Is Arguement able or not
        private bool _ArguementAble;

        // get and set command
        public bool ArguementAble
        {
            get
            { return _ArguementAble;}
            set
            {
                if (value == false || value == true)
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

    // Function to toggle tags and check for validity
    static Dictionary<string, bool> TagsChecker(Instructions I, string[] Tags)
    {
        Dictionary<string, bool> ValidTags = new();
        
        // Initialize all tags to false
        foreach (string tag in Tags)
        {
            ValidTags[tag] = false;
        }

        // If the Intrsuctions is not tagable and has tags, return an error
        if (I.TagAble == false && I.TagCount > 0)
        {
            I.ErrMessage = "Not a Flagable";
            I.ErrCode = 1;
            return ValidTags;
        }
        else
        {
            // Check for valid tags and toggle them
            foreach (string tag in I.TagsID)
            {
                if (!ValidTags.ContainsKey(tag))
                {
                    I.ErrMessage = "Invalid Tags";
                    I.ErrCode = 1;
                    return ValidTags;
                }

                ValidTags[tag] = true;
            }
        }

        return ValidTags;
    }

    static void ArgumentsChecker(Instructions I, int MinArgs, int MaxArgs)
    {
        if (!I.ArguementAble && I.ArgumentsCount > 0)
        {
            I.ErrMessage = "Not an Arguementable!";
            I.ErrCode = 1;
            return;
        }

        if (I.ArgumentsCount < MinArgs)
        {
            I.ErrMessage = "Too few Arguments!";
            I.ErrCode = 1;
            return;
        }

        if (MaxArgs != -1 && I.ArgumentsCount > MaxArgs)
        {
            I.ErrMessage = "Too many Arguments!";
            I.ErrCode = 1;
        }
    }

    static void ErrorProvider(Instructions I, string errmess, int errcode = 1)
    {
        I.ErrMessage = errmess;
        I.ErrCode = errcode;
        return;
    }

    static void ShowFolders(Instructions I)
    {
        // Tags toggler and checker
        var Tags = TagsChecker(I, ["-h", "-f", "-a"]);

        // Toggle variables for tags
        bool showAll = Tags["-a"];
        bool showFiles = Tags["-f"];
        bool showHidden = Tags["-h"];

        // Arguments Count checker
        ArgumentsChecker(I, 0, 1); // minargs = 0, maxargs = 1

        // If there is an error in the instructions, return early
        if (I.ErrCode == 1)
            return;

        // Determine the directory to show based on the arguments
        string dir = 
            (I.ArguementAble == true && I.ArgumentsCount == 1) 
            ? I.Arguments[0] : Directory.GetCurrentDirectory();

        try 
        {
            string[] folders;
            
            // If showFiles is true, get files, otherwise get directories
            folders = (showFiles) ? Directory.GetFiles(dir) : Directory.GetDirectories(dir);
            
            // If showAll is true, get both files and directories
            folders = (showAll) ? Directory.GetFileSystemEntries(dir) : folders;

            // If the folders array is null, use an empty array instead, and filter out any null or whitespace entries
            folders = (folders ?? Array.Empty<string>()).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();

            // bool to check whether anything has printed or not
            bool hasPrinted = false;

            // If folders is null, use an empty array instead, use ?? for nullable coalescing operator
            foreach (string? folder in (folders))
            {
                // Check whether the current folder is hidden or not
                if ((!((File.GetAttributes(folder) & FileAttributes.Hidden) != 0)) || (((File.GetAttributes(folder) & FileAttributes.Hidden) != 0) && showHidden))
                {
                    // Print the folder name with a prefix ".\" if it's a directory
                    Console.WriteLine(
                        (File.GetAttributes(folder) & FileAttributes.Directory) != 0 
                        ? ".\\" + Path.GetFileName(folder) 
                        : Path.GetFileName(folder)
                    );
                    hasPrinted = true;
                }
                else
                {
                    // If the folder is hidden and showHidden is false, skip printing it
                    continue;
                }
            }

            if (!hasPrinted)
            {
                if (showFiles)
                    Console.WriteLine((showHidden) ? "No hidden files" : "No files");
                else if (showAll)
                    Console.WriteLine((showHidden) ? "No hidden files or folders" : "No files or folders");
                else
                    Console.WriteLine((showHidden) ? "No hidden folders" : "No folders");
            }
        }
        catch(DirectoryNotFoundException)
        {
            ErrorProvider(I, "No such directory: " + Path.GetFullPath(I.Arguments[0]));
        }
        catch (Exception em)
        {
            ErrorProvider(I, em.Message);
        }        
    }

    static void Help(Instructions I)
    {
        // Tags toggler and checker
        var Tags = TagsChecker(I, []);

        if (I.ErrCode == 1)
            return;

        // Use "Bettle" in condition of nothing passed
        string help_cmd = I.ArgumentsCount > 0 ? I.Arguments[0] : "Bettle";
        string Errmessage = "";

        switch (help_cmd)
        {
            case "show":
                Errmessage = 
                    "show: \n" +
                    " tags: -h -f -a \n" + 
                    " >>> show: displays all folders or files present on the current directory \n" + 
                    " >>> show <FilePath>: displays all folders or files present on the file path written \n" + 
                    " -h: is for hidden directories or files but normal directories or files are also displayed \n" + 
                    " -f: is for files \n" + 
                    " -a: is for displaying both folders and files \n" + 
                    " Tags can be joined as -h -f means both hidden and files which mean hidden files and non - hidden files";

                break;

            case "cd":
                Errmessage =
                    "cd: \n" +
                    " tags: null\n" +
                    " >>> cd : displays the absolute path of the current directory\n" +
                    " >>> cd <FilePath>`: changes the cd to that FilePath, and if the FilePath does not exists ask the user to create one or not";

                break;
            
            case "create":
                Errmessage =
                    "create: \n" +
                    " tags: -h -f\n" +
                    " -h: is for hidden directories or file\n" +
                    " -f: is for file\n" +
                    " >>> create <Folder_Name>: create a folder with name specified\n" +
                    " Wild - Card Entry: & could be used to create a file while having requested for folders and vice versa\n" +
                    " \"Hello\\h1\\h2\": command is able to create a folder naming `Hello` and then `h1` under `Hello` and then `h2` under `h1` \n" +
                    " Tags can be joined as -h -f means both hidden and files which mean hidden file\n" +
                    " >>> create &&Hello.py creates a file named &Hello.py\n" +
                    " The current code can not create `&Hello` and `&Hello.py` at once in one line";

                break;

            case "murder":
                Errmessage =    
                    "murder: \n" + 
                    " tags: -f\n" +
                    " >>> murder <Folder_Name>: rmeove the folder with name specified\n" +
                    " Wild - Card Entry: & could be used to remove a file while having requested for folders and vice versa\n" +
                    " -f: is for file\n" +
                    " Hello\\h1\\h2`: command is able to remove a folder naming Hello and then h1 under Hello and then h2 under h1";

                break;

            case "stage":
                Errmessage = 
                    "stage: \n" +
                    " tags: -s, -c, -r\n" +
                    " stage <File_Path>: include the file name at the stage beforing checking for any repition\n" +
                    " -s: display all the files present at the stage\n" +
                    " -c: clears all the files present at the stage\n" +
                    " -r: remove the file or folder from the stage\n" +
                    " Remember we can also stage a file or folder not existing at all, so we can stage them";

                break;

            case "move":
                Errmessage = 
                    "move: \n" +
                    " tags: -r\n" +
                    " move <Source_path> <Destination_path>: move the directory from source path to dest. path\n" +
                    " move &<File_path> <Destination_path>: move the file from source path to dest. path\n" +
                    " move &<File_path> &<Destination_path>: rename the file from file path to dest. path\n" +
                    " -r: Will be used to destroy the file present at the destination";

                break;

            case "Bettle":
                Errmessage = "Available commands:\n" + 
                " show,\n" + 
                " help,\n" + 
                " cd,\n" + 
                " create,\n" + 
                " murder,\n" + 
                " stage,\n" + 
                " move";
                break;
        }

        Console.WriteLine(Errmessage);
    }

    static void CurrentDirectory(Instructions I)
    {
        // Tags toggler and checker
        var Tags = TagsChecker(I, []);

        // Arguements not less than 0 and not greater than 1
        ArgumentsChecker(I, 0, 1);

        // Checks for any error in TagsChecker and Arguements
        if (I.ErrCode == 1)
            return;

        // Checks whether any arguement is present or not
        else if (I.ArguementAble == true && I.ArgumentsCount == 1)
        {
            try {
                // Set the cd to the Arguments passed
                Directory.SetCurrentDirectory(I.Arguments[0]);
                // Display the cd
                Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
            }
            catch (DirectoryNotFoundException)
            {
                // Inquiring the user`bout whether to create the directory  
                Console.Write("Directory not found!\n Would you like to create the directory (y/n)? ");
                // Getting the response from the user trimming and lower-casing
                string? response = Console.ReadLine()?.Trim().ToLower();
                // if user enters Y, y, yes or Yes, Make the dir
                if (response == "y" || response == "yes")
                    MakeDirectory(I);
                else
                    return;
            }
            // Unauthorized access
            catch (UnauthorizedAccessException)
            {
                ErrorProvider(I, "Unauthorized access to file or directory!");
            }
            // IO Exception
            catch (IOException)
            {
                ErrorProvider(I, "I/O error occurred!");
            }
            catch (Exception em)
            {
                ErrorProvider(I, em.Message);
            }
        }
        else if (I.ArgumentsCount == 0)
        {
            // Getting the cd
            Console.WriteLine(Directory.GetCurrentDirectory());
        }    
    }

    static void MakeDirectory(Instructions I)
    {
        // Tags toggler and checker
        var Tags = TagsChecker(I, ["-h", "-f"]);

        // Toggle variables for tags
        bool ToggleHiddenDirectory = Tags["-h"];
        bool ToggleFile = Tags["-f"];

        // Arguments Count checker
        ArgumentsChecker(I, 1, -1); // minargs = 1, maxargs = INF

        // If there is an error in the instructions, return early
        if (I.ErrCode == 1)
            return;

        // This is from where the commands starts
        for (int i = 0; i < I.ArgumentsCount; i++)
        {
            try
            {
                // Wild - card entry "&" (for creating a file) for subString index
                int wildCard = I.Arguments[i].StartsWith("&") ? 1 : 0;

                // File is set to Full Path of args[i]
                string file = Path.GetFullPath(I.Arguments[i].Substring(wildCard));

                // Creation of File on ToggleFile or WildCard
                if (ToggleFile ^ (wildCard == 1))
                {
                    // Create File if not already exists
                    if (!File.Exists(file)) File.Create(file).Dispose();
                    Console.WriteLine(I.Arguments[i].Substring(wildCard) + (File.Exists(file) ? " Already Exists!" : " Created!"));
                }
                else
                {
                    // Create Dir if not already exists
                    if (Directory.Exists(file))
                        Console.WriteLine(file + " Already Exists!");                        
                    else
                        Console.WriteLine(Directory.CreateDirectory(file) + " created!");
                }

                // Hide if requested
                if (ToggleHiddenDirectory)
                    File.SetAttributes(file, FileAttributes.Hidden);
                
            }
            catch (UnauthorizedAccessException)
            {
                ErrorProvider(I,"Unauthorized access to file or directory");
            }
            catch (IOException)
            {
                ErrorProvider(I,"I/O error occurred");
            }
            catch(Exception em)
            {
                ErrorProvider(I, em.Message);
            }
        }
    }

    static void RemoveDirectory(Instructions I)
    {
        // Tags toggler and checker
        var Tags = TagsChecker(I, ["-f"]);

        // Toggle variables for tags
        bool ToggleFile = Tags["-f"];

        // Arguments Count checker
        ArgumentsChecker(I, 1, -1); // minargs = 1, maxargs = INF

        // If there is an error in the instructions, return early
        if (I.ErrCode == 1)
            return;

        for (int i = 0; i < I.Arguments.Count; i++)
        {
            try
            {
                // Wild - Card entry "&"
                if (I.Arguments[i].StartsWith("&"))
                {
                    // Delete the folder or file
                    if (ToggleFile == false)
                    {
                        if (File.Exists(I.Arguments[i].Substring(1)))
                        {
                            // Delete the files
                            File.Delete(I.Arguments[i].Substring(1));
                            Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i].Substring(1) + " is deleted successfully!");
                        }
                        else
                        {
                            ErrorProvider(I, "File does not exists");
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(I.Arguments[i].Substring(1)))
                        {
                            ErrorProvider(I, "Folder does not exists");
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
                    if (ToggleFile == true)
                    {
                        // Delete the files
                        if (File.Exists(I.Arguments[i]))
                        {
                            File.Delete(I.Arguments[i]);
                            Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + I.Arguments[i] + " is deleted successfully!");
                        }
                        else
                        {
                            ErrorProvider(I, "File does not exists");
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(I.Arguments[i]))
                        {
                            ErrorProvider(I, "Folder does not exists");
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
                ErrorProvider(I, "File does not exists");
            }
            catch (UnauthorizedAccessException)
            {
                ErrorProvider(I, "Unauthorized access to file or directory");
            }
            catch (IOException)
            {
                ErrorProvider(I, "I/O error occurred");
            }
            catch (Exception em)
            {
                ErrorProvider(I, em.Message);
            }
        }
    }

    static void Stage(Instructions I)
    {
        // Tags toggler and checker
        var Tags = TagsChecker(I, ["-s", "-c", "-m", "-r", "-d"]);

        if (I.ErrCode == 1)
            return;

        bool ToggleShow = Tags["-s"];
        bool ToggleClear = Tags["-c"];
        bool ToggleMake = Tags["-m"];
        bool ToggleRemove = Tags["-r"];
        bool ToggleDelete = Tags["-d"];

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
        // Tags toggler and checker
        var Tags = TagsChecker(I, ["-r"]);

        if (I.ErrCode == 1)
            return;

        bool ToggleRemove = Tags["-r"];

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
                        I.ArguementAble = false;
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
        I.TagAble = CurrentCommand?.TagAble ?? false;
        I.ArguementAble = CurrentCommand?.ArgumentAble ?? false;
        I.TagCount = TagCount;
        I.ArgumentsCount = ArgumentsCount;
        I.CommandObject = CurrentCommand;

        return I;
    }

    static void TakeActions(Instructions I)
    {
        if (I.CommandObject == null)
            return;

        if (I.ErrCode == 0)
        {
            try
            {
                I.CommandObject.Command(I);
            }
            catch (Exception ex)
            {
                I.ErrMessage = ex.Message;
                I.ErrCode = 1;
            }
        }

        if (I.ErrCode == 1)
            Console.WriteLine(I.ErrMessage);
    }

    static void Main()
    {
        TakeCommands();
    }

}