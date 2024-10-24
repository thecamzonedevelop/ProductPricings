using System.ComponentModel.DataAnnotations;

namespace ProductLib.Models;

public class ProductUpdateReq
{
    [Required(ErrorMessage = "Key is required")]
    public string Key { get; set; } = default!;
    public string? Name { get; set; } = default;
    public string? Category { get; set; } = default;
}