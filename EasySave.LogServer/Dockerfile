FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app

COPY bin/Release/net10.0/ .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EasySave.LogServer.dll"]