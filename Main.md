So we have some commands as:

- show

- help

- cd 

- create

straightforward as written

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

    `Hello\h1\h2`: command is able to create create a folder naming `Hello` and then `h1` under `Hello` and then `h2` under `h1` 

    Tags can be joined as `-h -f` means both hidden and files which mean hidden file
