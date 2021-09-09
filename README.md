# database-chatter

## Install Nuget Package

First, install the VS Code Extension:  'Nuget Package Manager'

Second, use ctrl + shift + p to pull up command pallet and use Nuget Package Manager: Add Package to install:

- System.Data.SqlClient
- Serilog.Sinks.File

Third, run the following command

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
