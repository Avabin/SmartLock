FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Source/SmartLock.SignalR/SmartLock.SignalR.csproj", "Source/SmartLock.SignalR/"]
RUN dotnet restore "Source/SmartLock.SignalR/SmartLock.SignalR.csproj"
COPY . .
WORKDIR "/src/Source/SmartLock.SignalR"
RUN dotnet build "SmartLock.SignalR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartLock.SignalR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartLock.SignalR.dll"]
