using System.ComponentModel.DataAnnotations;

namespace OrderFiltering.ViewModels;

public class DistrictViewModel
{
    [Required]
    public string Name { get; set; }
}