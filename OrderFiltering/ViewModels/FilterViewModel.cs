using System.ComponentModel.DataAnnotations;

namespace OrderFiltering.ViewModels;

public class FilterViewModel
{
    [Required] 
    public Guid DistrictId { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
    
    public string? FilterResultOutputType { get; set; }
}