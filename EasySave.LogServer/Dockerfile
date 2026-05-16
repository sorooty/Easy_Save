FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app

COPY bin/Release/net10.0/ .

EXPOSE 5275

ENV ASPNETCORE_URLS=http://+:5275

ENTRYPOINT ["dotnet", "EasySave.LogServer.dll"]