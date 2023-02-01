// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using ObjectDetector.Yolo.Onnx;

await Host.CreateDefaultBuilder(args).ConfigureServices((context, collection) =>
{
    collection.AddYoloOnnxObjectDetector(false);
}).RunConsoleAppFrameworkAsync<ImageRunner.ImageRunner>(args);