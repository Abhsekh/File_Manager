Commands:

- show

- help

- cd 

- create

1. show: 

    `tags: -h -f -a`

    `show`: displays all folders or files present on the current directory

    `show <FilePath>`: displays all folders or files present on the file path written

    `-h`: is for hidden directories or files but normal directories or files are also displayed
    
    `-f`: is for files

    `-a`: is for displaying both folders and files

    Tags can be joined as `-h -f` means both hidden and files which mean hidden files and non - hidden files

2. cd: 

    `tags: null`

    `cd `: displays the absolute path of the current directory

    `cd <FilePath>`: changes the cd to that FilePath, and if the FilePath does not exists ask the user to create one or not

3. create: 

    `tags: -h -f`

    `create <Folder_Name>`: create a folder with name specified

    `Wild - Card Entry`: `&` could be used to create a file while having requested for folders and vice versa

    `-h`: is for hidden directories or file
    
    `-f`: is for file

    `Hello\h1\h2`: command is able to create a folder naming `Hello` and then `h1` under `Hello` and then `h2` under `h1` 

    Tags can be joined as `-h -f` means both hidden and files which mean hidden file

    `create -f &&Hello` creates a folder named &Hello
    `create &&Hello.py` creates a file named &Hello.py

    The current code can not create `&Hello` and `&Hello.py` at once in one line

4. remove: 

    `tags: -f`

    `murder <Folder_Name>`: rmeove the folder with name specified

    `Wild - Card Entry`: `&` could be used to remove a file while having requested for folders and vice versa
    
    `-f`: is for file

    `Hello\h1\h2`: command is able to remove a folder naming `Hello` and then `h1` under `Hello` and then `h2` under `h1` 

5. stage:

    `tags: -s, -c, -r`

    `stage <File_Path>`: include the file name at the stage beforing checking for any repition

    `-s`: display all the files present at the stage

    `-c`: clears all the files present at the stage

    `-r`: remove the file or folder from the stage

    Remember we can also stage a file or folder not existing at all, so we can stage them

6. move:

    `tags: -r`

    `move <Source_path> <Destination_path>`: move the directory from source path to dest. path

    `move &<File_path> <Destination_path>`: move the file from source path to dest. path
    
    `move &<File_path> &<Destination_path>`: rename the file from file path to dest. path

    `-r`: Will be used to destroy the file present at the destination