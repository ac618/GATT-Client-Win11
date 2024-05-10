# GATT-Client-Win11
A GATT client implementation on Windows 11.

## Prerequisite
### .NET8.0
* link: https://dotnet.microsoft.com/en-us/download
* check versions: `dotnet --list-sdks`

## Run Project
`dotnet run`

## Initialize Project
This part is done already in this repo, this is only a tutorial for those who want to start a similar project from scratch. The steps avoids the trouble of failing to use Windows.Devices API.
1. Generate Project
    `dotnet new console --framework net8.0 --use-program-main`
2. Project Settings
    modify `<TargetFramework>` in the `.csproj` file to `<TargetFramework>net8.0-windows$([Microsoft.Build.Utilities.ToolLocationHelper]::GetLatestSDKTargetPlatformVersion('Windows', '11.0'))</TargetFramework>`
3. Add Package
    `dotnet add package Microsoft.Windows.CsWinRT --version 2.0.7`
