﻿# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /

# copy csproj and restore as distinct layers
COPY *.sln .
COPY . .
RUN dotnet restore

# copy everything else and build app
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MessagePublisher.WebApi.dll"]