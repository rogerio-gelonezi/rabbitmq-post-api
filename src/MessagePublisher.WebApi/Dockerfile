FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MessagePublisher.WebApi/MessagePublisher.WebApi.csproj", "src/MessagePublisher.WebApi/"]
COPY ["src/MessageBus.Engine/MessageBus.Engine.csproj", "src/MessageBus.Engine/"]
RUN dotnet restore "src/MessagePublisher.WebApi/MessagePublisher.WebApi.csproj"
COPY . .
WORKDIR "/src/src/MessagePublisher.WebApi"
RUN dotnet build "MessagePublisher.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessagePublisher.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessagePublisher.WebApi.dll"]
