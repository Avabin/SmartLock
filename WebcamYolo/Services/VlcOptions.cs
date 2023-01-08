using System.ComponentModel.DataAnnotations;

namespace WebcamYolo.Services;

public class VlcOptions
{
    [Required]
    public string VlcPath { get; set; }
}