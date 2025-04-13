rem https://github.com/microsoft/CsWin32/issues/1188

dotnet publish ".\LiveAppsOverlay\LiveAppsOverlay.csproj" -f net8.0-windows -r win-x64 -o E:\LiveAppsOverlay -p:PublishSingleFile=true --no-self-contained -p:PublishReadyToRun=false
dotnet publish ".\LiveAppsOverlay.Updater\LiveAppsOverlay.Updater.csproj" -f net8.0-windows -r win-x64 -o E:\LiveAppsOverlay -p:PublishSingleFile=true --no-self-contained -p:PublishReadyToRun=false