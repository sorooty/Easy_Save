FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

COPY bin/Release/net8.0/ .

EXPOSE 5275

ENTRYPOINT ["dotnet", "EasySave.LogServer.dll"]	