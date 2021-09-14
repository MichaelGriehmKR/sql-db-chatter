# sql-db-chatter

## Install the .NET Core SDK

Download and install the .NET Code SDL from [here](https://dotnet.microsoft.com/download).

## Install VS Code Extensions

First, install the following VS Code Extensions:  

- Nuget Package Manager
- C#

## Restore Nuget Packages

Open the terminal at the root of this directory and then run the following command:

    dotnet restore

### Fixing Nuget Package Manager Issue

I ran into an issue with the 'Nuget Package Manager' VS Code extension, but I used the following links to fix the issue:

- <https://stackoverflow.com/questions/58108809/versioning-information-could-not-be-retrieved-from-the-nuget-package-repository>
- <https://www.icode9.com/content-4-983377.html>

The solution was to make a change to the following file:

C:\Users\sog8543\.vscode\extensions\jmrog.vscode-nuget-package-manager-1.1.6\out\src\actions\add-methods\fetchPackageVersions.js

Change line 15 from:

    node_fetch_1.default(`${versionsUrl}${selectedPackageName}/index.json`, utils_1.getFetchOptions(vscode.workspace.getConfiguration('http')))

To:

    node_fetch_1.default(`${versionsUrl}${selectedPackageName.toLowerCase()}/index.json`, utils_1.getFetchOptions(vscode.workspace.getConfiguration('http')))

## Debugging

Add the following files and content to the `.vscode` folder in your Repo's root directory:

launch.json

    {
        // Use IntelliSense to learn about possible attributes.
        // Hover to view descriptions of existing attributes.
        // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
        "version": "0.2.0",
        "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net5.0/DatabaseChatter.ConsoleApp.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "console": "internalConsole"
        }
        ]
    }

tasks.json

    {
        "version": "2.0.0",
        "tasks": [
            {
                "label": "build",
                "command": "dotnet",
                "type": "process",
                "args": [
                    "build",
                    "${workspaceFolder}/DatabaseChatter.ConsoleApp.csproj"
                ],
                "problemMatcher": "$msCompile"
            }
        ]
    }
