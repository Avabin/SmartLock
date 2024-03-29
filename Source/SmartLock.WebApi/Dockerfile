﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Source/SmartLock.WebApi/SmartLock.WebApi.csproj", "Source/SmartLock.WebApi/"]
COPY ["Source/SmartLock.Client/SmartLock.Client.csproj", "Source/SmartLock.Client/"]
COPY ["Source/Yolov8.Client/Yolov8.Client.csproj", "Source/Yolov8.Client/"]
COPY ["Source/SmartLock.Config/SmartLock.Config.csproj", "Source/SmartLock.Config/"]
COPY ["Source/SmartLock.Orleans.Core/SmartLock.Orleans.Core.csproj", "Source/SmartLock.Orleans.Core/"]
COPY ["Source/SmartLock.Streams.RabbitMQ/SmartLock.Streams.RabbitMQ.csproj", "Source/SmartLock.Streams.RabbitMQ/"]
COPY ["Source/SmartLock.GrainInterfaces/SmartLock.GrainInterfaces.csproj", "Source/SmartLock.GrainInterfaces/"]
COPY ["Source/SmartLock.ObjectStorage/SmartLock.ObjectStorage.csproj", "Source/SmartLock.ObjectStorage/"]
RUN dotnet restore "Source/SmartLock.WebApi/SmartLock.WebApi.csproj"
COPY . .
WORKDIR "/src/Source/SmartLock.WebApi"
RUN dotnet build "SmartLock.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartLock.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartLock.WebApi.dll"]
