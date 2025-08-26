#include "CodeDependencies.iss"
[Setup]
AppName=Photobox
AppVersion=Beta-0.1.0
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={pf64}\Photobox
DefaultGroupName=Photobox
OutputDir=output
OutputBaseFilename=PhotoboxInstaller
Compression=lzma
SolidCompression=yes

[Files]
Source: "bin\Release\net9.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Photobox"; Filename: "{app}\Photobox.exe"
Name: "{commondesktop}\Photobox"; Filename: "{app}\Photobox.exe"

[Run]
Filename: "{app}\Photobox.exe"; Description: "Launch Photobox"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet9Installed(): Boolean;
var
  Versions: Array of String;
  I: Integer;
begin
  Result := False;

  if RegGetSubkeyNames(HKLM64,
    'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App',
    Versions) then
  begin
    for I := 0 to GetArrayLength(Versions) - 1 do
    begin
      if Pos('9.', Versions[I]) = 1 then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
begin
  if not IsDotNet9Installed() then
  begin
    MsgBox('Photobox requires the .NET 9 Desktop Runtime (x64). Please install it before running the application.',
           mbError, MB_OK);

    { Open Microsoft download page for .NET 9 runtime }
    ShellExec('', 'https://dotnet.microsoft.com/en-us/download/dotnet/9.0/runtime',
              '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);

    Result := False; { Abort setup }
  end
  else
    Result := True;
end;
