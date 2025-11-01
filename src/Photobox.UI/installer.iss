#include "CodeDependencies.iss"
[Setup]
AppName=Photobox
AppVersion=Beta-0.1.0
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={commonpf64}\Photobox
DefaultGroupName=Photobox
OutputDir=output
OutputBaseFilename=PhotoboxInstaller
Compression=lzma
SolidCompression=yes

[Files]
Source: "bin\Release\net9.0-windows\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Photobox"; Filename: "{app}\Photobox.exe"
Name: "{commondesktop}\Photobox"; Filename: "{app}\Photobox.exe"

[Run]
Filename: "{app}\Photobox.exe"; Description: "Launch Photobox"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup: Boolean;
begin
  // add the dependencies you need
  Dependency_AddDotNet90Desktop;
  // ...

  Result := True;
end;