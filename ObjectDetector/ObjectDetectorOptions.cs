using System.ComponentModel.DataAnnotations;

namespace ObjectDetector;

public class ObjectDetectorOptions
{
    [Required]
    public bool UseCuda { get; set; } = false;
}