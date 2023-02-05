using Moq;
using Tests.Shared.ClientConfigurators;
using Tests.Shared.SiloConfigurators;
using Yolov8.Client;

namespace SmartLock.Grains.Tests.Grains.DetectorGrain;

[Category("Unit"), TestFixture, Parallelizable(ParallelScope.Children)]
public class DetectorGrainTestsUnit : DetectorGrainTestsBase<UnitTestSiloBuilderConfigurator, UnitTestsClientConfigurator>
{
    [SetUp]
    public async Task SetUp()
    {
        var mock = UnitTestSiloBuilderConfigurator.YoloClientMock;

        mock.Setup(x => x.DetectAsync(It.Is<string>(s => s.Equals(CorrectUrl, StringComparison.InvariantCultureIgnoreCase)), It.IsAny<float>(), It.IsAny<float>()))
            .Returns(async () => new List<YoloDetectionModel>
            {
                new("dog", 0.9f, 1, 1, 1, 1),
            });

        mock.Setup(x => x.DetectAsync(It.Is<string>(s => s.Equals(NotImageUrl, StringComparison.InvariantCultureIgnoreCase)), It.IsAny<float>(), It.IsAny<float>()))
            .Throws<ArgumentException>();
        
        mock.Setup(x => x.DetectAsync(It.Is<string>(s => s.Equals(BadUrl, StringComparison.InvariantCultureIgnoreCase)), It.IsAny<float>(), It.IsAny<float>()))
            .Throws<InvalidOperationException>();
    }
}