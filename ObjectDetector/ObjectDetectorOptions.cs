using System.ComponentModel.DataAnnotations;

namespace Yolov7;

public class ObjectDetectorOptions
{
    [Required]
    public bool UseCuda { get; set; } = false;
}