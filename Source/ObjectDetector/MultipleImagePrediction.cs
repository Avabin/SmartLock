namespace ObjectDetector;

public record MultipleImagePrediction(TimeSpan Elapsed, List<SingleImagePrediction> Predictions);