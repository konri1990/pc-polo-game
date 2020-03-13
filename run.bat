@Echo Off
dotnet restore Server
sotnet restore Client
dotnet build Server
dotnet build Client
START cmd.exe /k "dotnet run --project Server"
START cmd.exe /k "dotnet run --project Client"
START cmd.exe /k "dotnet run --project Client"
PAUSE