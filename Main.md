# Understanding the Bettle File Manager — Step by Step

This guide explains the C# code for your command-line file manager in simple language. It focuses on **what each part does, why it exists, and how a command travels through the program**.

> Project idea: You type commands such as `show`, `show -f`, `pwd`, or `create notes`. The program interprets the text, checks whether the command is valid, and then calls the matching method.

---

## 1. Start with the big picture

The program has five main jobs:

1. **Register commands** — keep a list of commands the program understands.
2. **Read input** — display `Bettle >` and read what you type.
3. **Parse and validate** — split your input into a command, tags, and arguments.
4. **Run the action** — call the method associated with the command.
5. **Display a result** — show folders, create items, print the current directory, or explain a problem.

The flow looks like this:

```text
Main()
  |
  v
TakeCommands()
  | reads one line
  v
ValidateCommands(text)
  | creates and fills an Instructions object
  v
TakeActions(instructions)
  | finds the selected command's delegate
  v
ShowFolders / Help / CurrentDirectory / MakeDirectory
```

The important design idea is that **parsing a command and performing a command are separate jobs**. `ValidateCommands` tries to understand the input; `TakeActions` runs the selected operation.

---

## 2. The `using` statements

```csharp
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Transactions;
```

A `using` directive makes types from a namespace available without writing their full names.

- `System` provides types such as `Console`, `Action`, and basic language-supporting classes.
- `System.IO` provides file and directory operations, including `File`, `Directory`, `Path`, and `FileAttributes`.

The other three namespaces in this list are not visibly used in the code shown. You can usually remove unused `using` directives. Your code also uses `List<T>`, `FirstOrDefault`, and `StringSplitOptions`; depending on your project’s implicit/global usings, you may need:

```csharp
using System.Collections.Generic;
using System.Linq;
```

Modern .NET templates often include these through implicit usings.

---

## 3. The `Program` class

```csharp
class Program
{
    // The program's classes, data, and methods live here.
}
```

`Program` groups the code that makes up the application. Most methods are `static`, meaning you can call them from `Main` without first creating a `Program` object.

The code also declares two classes inside `Program`:

- `CommandClass`: describes a command the application supports.
- `Instructions`: stores the result of parsing one line of user input.

Nested classes are allowed in C#. They are being used here to keep these types close to the program that uses them.

---

## 4. `CommandClass`: describing a command

```csharp
public class CommandClass
{
    public int Id;
    public string Name;
    public bool ArgumentAble;
    public bool TagAble;
    public List<string> TagsId = new();
    public Action<Instructions> Command;
}
```

One `CommandClass` object represents one available command.

| Member | Meaning |
|---|---|
| `Id` | Numeric identifier for the command |
| `Name` | Text users type, such as `"show"` |
| `ArgumentAble` | Whether the command is intended to accept arguments |
| `TagAble` | Whether the command is intended to accept tags such as `-f` |
| `TagsId` | The tags allowed by the command’s definition |
| `Command` | The method to call when the command runs |

### What is `Action<Instructions>`?

`Action<T>` is a .NET **delegate type**. A delegate stores a reference to a method with a compatible signature.

Here, `Action<Instructions>` means:

- the method accepts one `Instructions` object;
- the method returns no value (`void`).

For example, `ShowFolders(Instructions I)` has a compatible signature, so it can be stored in a command object.

This lets the program use the same execution code for every command rather than writing a large `switch` statement for all command names.

### The constructor

```csharp
public CommandClass(
    int Id,
    string Name,
    bool ArgumentAble,
    bool TagAble,
    List<string> TagsId,
    Action<Instructions> cmd)
{
    this.Id = Id;
    this.Name = Name;
    this.ArgumentAble = ArgumentAble;
    this.TagAble = TagAble;
    this.TagsId = TagsId;
    this.Command = cmd;
}
```

A constructor runs when you create an object with `new`.

The parameters are values supplied by the caller. For example, `Name` is a constructor parameter, while `this.Name` refers to the object's field. The keyword `this` makes the distinction clear.

### Registering the commands

```csharp
static List<CommandClass> Commandslist =
[
    new CommandClass(1, "show", true, true, ["-h", "-f"], ShowFolders),
    new CommandClass(2, "help", true, false, [], Help),
    new CommandClass(3, "pwd", false, false, [], CurrentDirectory),
    new CommandClass(4, "create", true, true, ["-h", "-f"], MakeDirectory)
];
```

This is the command registry: a list of command descriptions.

| ID | Command | Accepts arguments? | Accepts tags? | Method |
|---:|---|---|---|---|
| 1 | `show` | Yes | Yes: `-h`, `-f` are listed | `ShowFolders` |
| 2 | `help` | Yes | No | `Help` |
| 3 | `pwd` | No | No | `CurrentDirectory` |
| 4 | `create` | Yes | Yes: `-h`, `-f` are listed | `MakeDirectory` |

The square-bracket collection expression is modern C# syntax. `[]` represents an empty collection in this context.

**Important distinction:** `TagsId` records the tags permitted by the command definition, but the current validation code does not check each supplied tag against this list. The command methods currently check whether a tag is `-h` or `-f`. A future improvement is to validate the supplied tags against `CurrentCommand.TagsId` centrally.

---

## 5. `Instructions`: the result of parsing one input line

The `Instructions` class is a data container. Imagine the user types:

```text
show -h C:\Temp
```

The program needs to remember details such as:

- Was a real command found?
- Which command was it?
- Were tags supplied?
- Which arguments were supplied?
- What message should be shown?
- Which `CommandClass` object describes the command?

That is the purpose of `Instructions`.

### 5.1 Assigning each instruction an ID

```csharp
static int CurrentId = 0;

public Instructions()
{
    _Id = ++CurrentId;
}
```

Every time `new Instructions()` is executed, the shared counter increases and its new value is assigned to `_Id`.

For example, the first object gets ID `1`, the next gets ID `2`, and so on. `static` means the counter belongs to the class, not to each individual object.

```csharp
private int _Id;

public int Id
{
    get { return _Id; }
}
```

The private field stores the value. The public property allows other code to read it, but not set it directly.

### 5.2 Fields and properties

Consider:

```csharp
private int _IsCommand;

public int IsCommand
{
    get { return _IsCommand; }
    set
    {
        if (value == 0 || value == 1)
            _IsCommand = value;
    }
}
```

- `private` prevents unrelated code from accessing the backing field directly.
- `get` returns the stored value.
- `set` receives a proposed new value through the special keyword `value`.
- The condition accepts only `0` or `1`.

This property represents a Boolean idea using an `int`: `0` means false and `1` means true. In C#, an actual `bool` is usually clearer, but keeping `int` can be useful if you intentionally want these numeric values.

The same validation pattern is used by `CommandID`, `TagAble`, `ArguementAble`, `TagCount`, and `ArgumentsCount`:

- `CommandID` accepts values at least `-1`; `-1` means no command was found.
- `TagAble` and `ArguementAble` accept only `0` or `1`.
- `TagCount` and `ArgumentsCount` accept zero or greater.

A setter that rejects an invalid value silently leaves the old value unchanged. That can make mistakes hard to notice; throwing an exception or returning a validation result may be clearer in larger programs.

### 5.3 Collections for tags and arguments

```csharp
private List<string> _TagsID = new();

public List<string> TagsID
{
    get { return _TagsID; }
    set { _TagsID = value; }
}

private List<string> _Arguements = new();

public List<string> Arguements
{
    get { return _Arguements; }
    set { _Arguements = value; }
}
```

A `List<string>` stores a changeable sequence of strings.

For example, the tags list might contain `"-h"` and `"-f"`, while the arguments list might contain `"C:\\Temp"`.

`new()` creates an empty list. The code later calls `.Add(...)` to append items and `.Clear()` to remove all items.

The spelling `Arguements` is misspelled. C# allows it, so the program can still compile, but consider renaming it to `Arguments` consistently throughout the code.

### 5.4 The message and command reference

```csharp
public string Message = "";
public CommandClass? CommandObject;
```

- `Message` stores a message such as `"Not a Command"` or `"Invalid Tags"`.
- `CommandObject` refers to the command description selected during parsing.
- `?` means the reference is allowed to be `null`. If the input does not match a command, there may be no command object.

A key idea: **the instruction object carries information from the parser to the command method**.

---

## 6. `Main`: where execution starts

```csharp
static void Main()
{
    TakeCommands();
}
```

When the program starts, .NET calls `Main`. This program immediately hands control to `TakeCommands`, which runs the interactive command loop.

---

## 7. `TakeCommands`: reading commands repeatedly

```csharp
static void TakeCommands()
{
    string? command;
    do
    {
        Console.Write("Bettle > ");
        command = Console.ReadLine();

        if (command == "exit" || command == "quit" || command == "kill")
            break;

        if (command != null)
            TakeActions(ValidateCommands(command));
        else
            continue;
    }
    while (true);

    Console.WriteLine("Bettle Closing!");
}
```

Step by step:

1. Print the prompt.
2. Read a line from the keyboard.
3. If it is exactly `exit`, `quit`, or `kill`, leave the loop.
4. If a line was received, validate it.
5. Pass the resulting `Instructions` object to `TakeActions`.
6. Repeat.

`string?` indicates that `command` may be `null`. `Console.ReadLine()` can return `null` when input ends.

### A small improvement

The exit checks are case-sensitive and do not trim extra spaces. Therefore `EXIT` or ` exit ` will not match. You could normalize the input before checking it, for example:

```csharp
command = Console.ReadLine()?.Trim();

if (command is not null &&
    (command.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
     command.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
     command.Equals("kill", StringComparison.OrdinalIgnoreCase)))
{
    break;
}
```

If you use this, make sure the rest of the loop still handles `null` appropriately.

---

## 8. `ValidateCommands`: turning text into structured data

This is the central parsing method. It does not normally create folders or list files. It attempts to interpret the user's text and fill an `Instructions` object.

### 8.1 Create a fresh instruction object

```csharp
Instructions I = new();
```

Each input line gets a new object, so tags, arguments, and messages from the previous line do not carry over.

### 8.2 Normalize repeated spaces

```csharp
string[] _Parts = command.Split(
    ' ',
    StringSplitOptions.RemoveEmptyEntries);

command = string.Join(" ", _Parts);
```

If the input is:

```text
     Show   -h    folder
```

the first line splits it into non-empty pieces:

```text
["Show", "-h", "folder"]
```

`string.Join` joins the pieces with one space:

```text
"Show -h folder"
```

This is useful because users may accidentally type extra spaces.

Then:

```csharp
string[] Parts = command.Split(" ");
```

splits the normalized command into an array. For the example:

| Index | Value |
|---:|---|
| `Parts[0]` | `"Show"` |
| `Parts[1]` | `"-h"` |
| `Parts[2]` | `"folder"` |

**Potential edge case:** if the input is empty or contains only spaces, `Split(..., RemoveEmptyEntries)` produces an empty array. Accessing `Parts[0]` then throws an `IndexOutOfRangeException`. Check `_Parts.Length == 0` before accessing the first element.

### 8.3 Set the initial parsing values

```csharp
int ArgumentsCount = 0;
int TagCount = 0;
int IsCommand = 0;
int CommandID = -1;
CommandClass? CurrentCommand = null;
```

These are temporary variables used while parsing:

- counts start at zero;
- `IsCommand` starts as false (`0`);
- `CommandID` starts at `-1`, meaning unknown;
- `CurrentCommand` starts as `null`.

### 8.4 Find the command in the registry

```csharp
CurrentCommand =
    Commandslist.FirstOrDefault(c => c.Name == Parts[0].ToLower());
```

`FirstOrDefault` searches the list and returns the first item matching the condition, or the default value (`null` for a reference type) if nothing matches.

The lambda:

```csharp
c => c.Name == Parts[0].ToLower()
```

means “for each command `c`, check whether its name equals the first word converted to lowercase.”

So `Show` can match the registered name `show`.

When a match is found, the code stores its ID. Otherwise, it sets the message to `"Not a Command"` and keeps `CommandID` at `-1`.

### 8.5 Find tags

The code treats words starting with `-` as tags. It first checks the first word after the command and then looks for consecutive words that both start with `-`.

For example:

```text
show -h -f
```

is intended to produce these tags:

```text
TagsID = ["-h", "-f"]
TagCount = 2
```

Then the code checks whether the command allows tags at all. If tags were supplied to a command whose `TagAble` setting is `false`, it sets:

```csharp
I.Message = "Not a Flagable";
```

**Important limitation:** this section is fragile. The parser uses `IsCommand` (which is `1` for every valid command) as an array index and uses `TagCount` to decide where arguments begin. This can misclassify input when arguments appear before tags or when tags are not consecutive. A clearer design is to examine every token after the command once, classify it as a tag or argument, and validate the two categories separately.

### 8.6 Find arguments

The code attempts to collect non-tag tokens into `I.Arguements`:

```csharp
if (!(Parts[i].StartsWith("-")))
{
    I.Arguements.Add(Parts[i]);
    ArgumentsCount++;
}
else
{
    I.Arguements.Clear();
    I.ArguementAble = 0;
    I.ArgumentsCount = 0;
    I.Message = "Invalid Arguement";
    break;
}
```

In simple terms:

- a token not beginning with `-` is treated as an argument;
- a token beginning with `-` in this part of the loop is treated as an invalid argument;
- when that happens, the argument list is cleared and a message is recorded.

After collecting arguments, the code checks whether the command is allowed to accept them. If not, it sets `"Not a Arguementable"`.

### 8.7 Quoted arguments and names beginning with `-`

A filename can begin with `-`, but the parser currently treats tokens beginning
with `-` as tags. For example, `create -f` is ambiguous: it could mean “create
a file” with no name, or “create an item named `-f`.”

A familiar way to express the filename is to put it in double quotes:

```text
create "-f"
create -f "-f"
create "meeting notes"
```

The intended interpretation is that quoted text is always an argument, even
when it begins with `-` or contains spaces. Unquoted recognized tags such as
`-f` remain tags. With the current `create` behavior, `create "-f"` would mean
“create a directory named `-f`,” while `create -f "-f"` would mean “create a
file named `-f`.”

**This syntax is not implemented by the current parser.** Splitting the input
with `string.Split` does not understand quotes; it leaves quote characters in
the tokens and cannot keep a quoted name containing spaces together. To support
this syntax, replace the simple split with a tokenizer that recognizes quoted
tokens and records that they are literal arguments. Then parse unquoted tokens
as tags only when they match a tag supported by the command, and validate
unknown tags separately.

### 8.8 Copy the results into `Instructions`

At the end, the method stores its local parsing results on the object:

```csharp
I.IsCommand = IsCommand;
I.CommandID = CommandID;
I.TagAble = (TagCount != 0) ? 1 : 0;
I.ArguementAble = (ArgumentsCount != 0) ? 1 : 0;
I.TagCount = TagCount;
I.ArgumentsCount = ArgumentsCount;
I.CommandObject = CurrentCommand;
```

The conditional expression:

```csharp
(condition) ? valueIfTrue : valueIfFalse
```

means “choose one value based on the condition.” Here, a nonzero tag count becomes `1`, otherwise `0`.

Finally:

```csharp
return I;
```

returns the filled instruction object to the caller.

**Design note:** `TagAble` and `ArguementAble` in `Instructions` currently mean “the input contained at least one tag/argument,” not “the command definition permits tags/arguments.” Those are different concepts. The command definition already has permission fields with the same names, so using different names such as `HasTags` and `HasArguments` in `Instructions` would reduce confusion.

---

## 9. `TakeActions`: dispatching the command

```csharp
static void TakeActions(Instructions I)
{
    if (I.CommandObject == null)
        return;

    I.CommandObject.Command(I);
}
```

This is where the stored delegate is invoked.

1. If no command matched, `CommandObject` is `null`, so the method returns.
2. Otherwise, it calls the method stored in that command object's `Command` field, passing the `Instructions` object.

For example, if the user types `pwd`, the parser finds the `CommandClass` object for `pwd`. That object stores `CurrentDirectory` as its delegate. `TakeActions` then calls:

```csharp
CurrentDirectory(I);
```

Conceptually, the code achieves this through the delegate instead of naming the method directly.

**Current limitation:** the code often stores errors in `I.Message`, but `TakeActions` does not display or process that message. As a result, some invalid inputs can appear to do nothing. A useful next step is to check `I.Message` after validation and show it before trying to execute the command.

---

## 10. `ShowFolders`: list directories or files

The method starts with:

```csharp
string dir = "";
int ToggleHiddenDirectories = 0;
int ToggleFiles = 0;
```

- `dir` will hold the path to inspect.
- `ToggleHiddenDirectories` controls whether hidden entries should be displayed.
- `ToggleFiles` switches between listing directories and listing files.
- `0` means off and `1` means on.

### 10.1 Read the tags

```csharp
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
        default:
            I.Message = "Invalid Tags";
            return;
    }
}
```

`foreach` visits every tag in the list. `switch` selects the matching case:

- `-h`: include hidden entries;
- `-f`: list files rather than directories;
- anything else: record an error and stop this method.

Note that `ShowFolders` checks the tag spellings itself. It does not use the allowed-tag list from `CommandClass`.

### 10.2 Check the number of arguments and choose a path

```csharp
if (I.ArgumentsCount > 1)
{
    I.Message = "To many Arguements";
    return;
}
else
{
    if (I.ArguementAble == 1 && I.ArgumentsCount == 1)
        dir = I.Arguements[0];
    else
        dir = Directory.GetCurrentDirectory();
}
```

The intended rule is:

- zero arguments: use the current working directory;
- one argument: use that path;
- more than one: reject the command.

`Directory.GetCurrentDirectory()` returns the process's current working directory. It is not necessarily the folder containing your executable.

### 10.3 Get the entries

```csharp
if (ToggleFiles == 0)
    folders = Directory.GetDirectories(dir);
else
    folders = Directory.GetFiles(dir);
```

- `Directory.GetDirectories(dir)` returns the paths of immediate subdirectories.
- `Directory.GetFiles(dir)` returns the paths of files directly inside the directory.

Neither recursively lists the contents of every subfolder.

If the result is empty, the program prints `"No folders"`. That message is slightly misleading when `-f` is active; `"No entries found"` would work for both modes.

### 10.4 Detect hidden entries

```csharp
FileAttributes attributes = File.GetAttributes(folder);
int HiddenFolder =
    ((attributes & FileAttributes.Hidden) != 0) ? 1 : 0;
```

`File.GetAttributes` retrieves metadata flags for the file-system entry. `FileAttributes.Hidden` is a bit flag. The `&` operator checks whether that particular flag is present.

- if the hidden flag is absent, the result is `0`;
- if it is present, the result is `1`.

The method prints an entry if it is not hidden, or if it is hidden and `-h` was supplied. `Path.GetFileName(folder)` prints only the final name rather than the full path.

Although the variable names say “folder,” the same attribute check is used when listing files.

### 10.5 Handle common file-system errors

```csharp
catch (DirectoryNotFoundException)
{
    Console.WriteLine("Directory does not exist.");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Access denied.");
}
```

The `try` block contains operations that can fail. A matching `catch` lets the program respond to known failures instead of terminating with an unhandled exception.

For example, a path may not exist, or Windows may deny access to it.

---

## 11. `CurrentDirectory`: implement `pwd`

```csharp
static void CurrentDirectory(Instructions I)
{
    if (I.TagAble == 1 || I.TagCount != 0 || I.TagsID.Count != 0)
    {
        I.Message = "Not a Flagable";
        return;
    }
    else if (I.ArguementAble == 1 ||
             I.ArgumentsCount != 0 ||
             I.Arguements.Count != 0)
    {
        I.Message = "Not a Arguementable";
        return;
    }
    else
    {
        Console.WriteLine(Directory.GetCurrentDirectory());
    }
}
```

The intended behavior is to print the current working directory, like the `pwd` command on Unix-like systems.

Before printing, it checks that no tags or arguments were supplied. It checks several related values because it is trying to catch inconsistent parser state too.

For instance, `pwd folder` should not be accepted because the registered `pwd` command does not allow arguments.

However, `Message` is only assigned here. Since the current `TakeActions` method does not print it, an invalid `pwd` input may not show the explanation. Centralized error reporting would solve this.

---

## 12. `MakeDirectory`: create folders and files

This method handles the `create` command. It uses two toggles:

```csharp
int ToggleHiddenDirectory = 0;
int ToggleFile = 0;
```

The intended interpretation is:

- `-h`: mark the created item as hidden;
- `-f`: switch the item type behavior.

It first checks the tags, then loops over the supplied arguments:

```csharp
for (int i = 0; i < I.Arguements.Count; i++)
```

This allows a command to create several items in one invocation, provided the parser accepts all the arguments.

### 12.1 Understand the `&` prefix

The current implementation uses `&` as a custom way to reverse whether
`create` makes a file or directory. It is separate from quoting: double quotes
are a clearer way to mark a name such as `-f` as a literal argument, but they
will only work after the parser is updated to understand quoted tokens (see
Section 8.7).

```csharp
if (I.Arguements[i].StartsWith("&"))
```

The code uses `&` as a custom marker. In the current logic, the marker changes the normal folder/file choice.

| Argument form | `-f` absent | `-f` present |
|---|---|---|
| `create notes` | Create a directory named `notes` | Create a file named `notes` |
| `create &notes` | Create a file named `notes` | Create a directory named `notes` |

The `&` itself is removed with:

```csharp
I.Arguements[i].Substring(1)
```

`Substring(1)` returns the string beginning at index `1`, so `&notes` becomes `notes`.

### 12.2 Build a full path safely

```csharp
file = Path.Combine(
    Directory.GetCurrentDirectory(),
    I.Arguements[i]);
```

`Path.Combine` joins path pieces using the appropriate separator. It is safer and clearer than manually concatenating a folder path and a file name.

If the argument is a rooted path, `Path.Combine` can return that rooted path rather than combining it with the current directory. If you want to allow only names in the current directory, you should validate that separately.

### 12.3 Create the item

```csharp
Directory.CreateDirectory(file);
```

Creates a directory. If the directory already exists, this method normally returns a `DirectoryInfo` for the existing directory rather than throwing an “already exists” exception.

```csharp
File.Create(file).Dispose();
```

Creates or overwrites a file and returns a `FileStream`. Calling `Dispose()` immediately closes the stream so the file is not left open. Be careful: `File.Create` truncates an existing file, so it can erase its contents.

### 12.4 Mark the item as hidden

```csharp
if (ToggleHiddenDirectory == 1)
{
    File.SetAttributes(file, FileAttributes.Hidden);
}
```

This sets the item's attributes to `Hidden`. Be aware that this assignment replaces the attribute set with `Hidden`; if you need to preserve existing attributes, combine the existing attributes with `FileAttributes.Hidden` instead.

Also, the variable is called `ToggleHiddenDirectory`, but this code applies the hidden attribute to files as well as directories.

### 12.5 Catch errors

```csharp
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Access denied.");
}
catch (IOException)
{
    Console.WriteLine("File Already Exists!");
}
```

The first handler reports a permissions problem. The second handles many I/O errors, but **not every `IOException` means the file already exists**. A clearer message would be `"Could not create the item: " + ex.Message`, or the code could check the specific condition it wants to handle.

---

## 13. `Help`: print a response for a message

```csharp
static void Help(Instructions I)
{
    string help_cmd = I.Message;
    string message = "";

    switch (help_cmd)
    {
        case "show":
            message = "Helping with the show!";
            break;
        case "Bettle":
            message = "Helping with the Bettle!";
            break;
        // More cases...
        default:
            message = "";
            break;
    }

    Console.WriteLine(message);
}
```

`Help` reads `I.Message`, compares it with known strings, and prints a matching response.

At the moment, this is not yet a complete general help system. For example, the parser does not automatically set `I.Message` to `"show"` when the user types `help show`, so that input may not behave as intended. A future design could give `Help` access to the argument list and print information about the requested command.

---

## 14. A complete example: `show -h C:\Temp`

Suppose you type:

```text
Bettle > show -h C:\Temp
```

Here is the intended journey:

1. `TakeCommands` reads the text.
2. `ValidateCommands` normalizes the spacing and splits the text into tokens.
3. It finds the command definition whose name is `show`.
4. It tries to identify `-h` as a tag and `C:\Temp` as an argument.
5. It stores the result in an `Instructions` object.
6. `TakeActions` retrieves the selected `CommandClass` object.
7. The object's delegate invokes `ShowFolders(I)`.
8. `ShowFolders` enables hidden entries, chooses `C:\Temp`, and gets the entries to list.
9. It prints each entry name that passes the hidden-entry check.

This describes the intended flow. The current parser's tag/argument indexing has edge cases, so test this exact example and improve the parser if it does not classify the tokens as expected.

---

## 15. Important issues to fix next

These are the most useful corrections to consider before adding more commands.

### 15.1 Empty input can crash the parser

Before using `Parts[0]`, handle a line that becomes an empty array after splitting:

```csharp
string[] parts = command.Split(
    ' ',
    StringSplitOptions.RemoveEmptyEntries);

if (parts.Length == 0)
{
    I.Message = "Empty command";
    return I;
}
```

Place this immediately after splitting, before any `parts[0]` access.

### 15.2 Messages are not centrally displayed

You often set `I.Message`, but `TakeActions` ignores it. Consider separating validation errors from informational messages and printing validation errors before running a command. For example, you could add an `IsValid` flag or return a result object containing success/failure and a message.

### 15.3 Tag validation is duplicated and incomplete

The command definitions list allowed tags, but the parser and command methods do not consistently validate against that list. A stronger design would:

1. parse every token after the command;
2. identify tag tokens and argument tokens;
3. reject unknown tags;
4. reject tags if the command does not support them;
5. reject arguments if the command does not support them;
6. pass only validated input to the action.

### 15.4 The tag/argument parser assumes a particular order

The current algorithm counts tags and uses that count to calculate where arguments begin. This breaks down when tags and arguments are interleaved or when an argument comes before a tag. Parse each token once instead of inferring token positions from counts.

### 15.5 `TagsId` should be used or removed

Each command stores a list of supported tags, but that list is not used by the parser. Either use it for validation or remove it to avoid having two sources of truth.

### 15.6 The file/directory creation logic is easy to reverse accidentally

The `&` prefix and `-f` flag interact to reverse the normal behavior. Keep the table in Section 12.1 nearby while testing. You might eventually prefer clearer flags such as `--file` and `--directory`.

### 15.7 Avoid overwriting files unintentionally

`File.Create(path)` can overwrite an existing file. If the goal is to create a new file only, check for an existing file first or use an API that opens the file with `FileMode.CreateNew`. Decide what behavior you want and report it clearly.

### 15.8 Some variable and property names can be clearer

Consider renaming:

- `Commandslist` → `Commands`
- `TagsId` → `AllowedTags`
- `Arguements` → `Arguments`
- `ArguementAble` → `AllowsArguments` in `CommandClass`
- the input-presence flags in `Instructions` → `HasTags` and `HasArguments`

Names should communicate whether a value describes a command's capabilities or what the user actually typed.

### 15.9 Check the meaning of `-f` in each command

In `show`, `-f` means “list files.” In `create`, it means “create a file unless `&` reverses the behavior.” Reusing a flag with related but different meanings can be confusing. This is a design decision rather than a C# syntax error.

---

## 16. A good order for learning this code

Study it in this order rather than trying to memorize every line:

1. **Objects and constructors:** understand how `new CommandClass(...)` creates command descriptions.
2. **Lists:** understand `List<CommandClass>`, `List<string>`, `.Add`, `.Clear`, and `.Count`.
3. **Delegates:** understand why `ShowFolders` can be stored in `Action<Instructions>`.
4. **Properties:** understand `get`, `set`, private backing fields, and validation.
5. **Parsing:** trace how one line becomes `Parts`, then tags and arguments.
6. **Dispatch:** follow `CommandObject.Command(I)` into the selected method.
7. **File-system APIs:** experiment with `Directory`, `File`, `Path`, and `FileAttributes`.
8. **Error handling:** learn what `try`, `catch`, and specific exception types do.
9. **Refactor the parser:** make it classify each token exactly once and report errors consistently.

### Practice exercise

Trace this input without running the program:

```text
show -f C:\Temp
```

Write down:

- the `Parts` array;
- which `CommandClass` should match;
- the expected tags;
- the expected arguments;
- which method should run;
- whether the method should list files or directories.

Then test it and compare the actual behavior with the intended behavior. That gap is a useful way to learn debugging and parser design.

---

## 17. The central idea to remember

The program is built around three different kinds of information:

- **`CommandClass`** describes what a command is and which method it runs.
- **`Instructions`** describes what the user typed and what the parser understood.
- **The command methods** perform the actual work on the file system or print information.

The registry lets you add a new command by defining its metadata and method, while the parser and dispatcher provide a shared route from keyboard input to execution. Keeping those responsibilities separate is the strongest architectural idea in this project.
