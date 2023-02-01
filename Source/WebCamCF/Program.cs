// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text.Json;
using FlashCap;
using WebCamCF;

var compreFaceUrl = "http://localhost:8000";
var http = new HttpClient()
{
    BaseAddress = new Uri(compreFaceUrl),
};
http.DefaultRequestHeaders.Add("x-api-key", "1cf57cf5-826f-442d-812f-92deeef36d7d");
Console.WriteLine("Searching webcams");
var devices = new CaptureDevices();
var deviceDescriptor = devices.EnumerateDescriptors().First();

Console.WriteLine("Opening webcam");
ISubject<byte[]> imageSubject = new ReplaySubject<byte[]>(1);
await using var device = await deviceDescriptor.OpenAsync(deviceDescriptor.Characteristics[0], async bufferScope =>
{
    var image = bufferScope.Buffer.ExtractImage();
    imageSubject.OnNext(image);
});
var intervalObservable = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.4));
var intervalImageObservable = intervalObservable.Select(_ => imageSubject.Take(1).ToTask().ToObservable()).Concat().Timestamp();
await device.StartAsync();
Console.WriteLine("Press any key to exit");
intervalImageObservable.Select(x => SendAsync(x.Value, x.Timestamp).ToObservable()).Concat().Subscribe();
Console.ReadKey();
async Task SendAsync(byte[] image, DateTimeOffset timestamp)
{
    var base64Image = Convert.ToBase64String(image);
    var content = new
    {
        file = base64Image
    };
    var plugins = "all";
    var predictionCount = 1;
    var limit = 10;
    var detProbThreshold = "0.7";
    var response = await http.PostAsJsonAsync($"/api/v1/recognition/recognize?limit={10}&det_prob_threshold={detProbThreshold}&prediction_count={predictionCount}&face_plugins={plugins}", content);
    var resultString = await response.Content.ReadAsStringAsync();
    if (resultString.Contains("No face is found in the given image"))
    {
        Console.WriteLine($"No face found at {timestamp:O}");
        return;
    }
    var root = JsonSerializer.Deserialize<RootObject>(resultString);
    var result = root.result[0];
    var subjects = result.subjects;
    foreach (var subject in subjects)
    {
        Console.WriteLine($"------ Subject | {subject.subject} | has <{subject.similarity}%> probability at time >>{timestamp:O}<<");
    }
}

namespace WebCamCF
{
    public record RootObject(
        Result[] result,
        Plugins_versions plugins_versions
    );

    public record Result(
        Box box,
        Subjects[] subjects,
        Execution_time execution_time
    );

    public record Box(
        double probability,
        int x_max,
        int y_max,
        int x_min,
        int y_min
    );

    public record Subjects(
        string subject,
        double similarity
    );

    public record Execution_time(
        double detector,
        double calculator
    );

    public record Plugins_versions(
        string detector,
        string calculator
    );
}