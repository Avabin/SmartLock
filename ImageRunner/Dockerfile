FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ImageRunner/ImageRunner.csproj", "ImageRunner/"]
COPY ["ObjectDetector.Yolo.Onnx/ObjectDetector.Yolo.Onnx.csproj", "ObjectDetector.Yolo.Onnx/"]
COPY ["ObjectDetector/ObjectDetector.csproj", "ObjectDetector/"]
COPY ["Yolo.Core/Yolo.Core.csproj", "Yolo.Core/"]
RUN dotnet restore "ImageRunner/ImageRunner.csproj"
COPY . .
WORKDIR "/src/ImageRunner"
RUN dotnet build "ImageRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageRunner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageRunner.dll"]
