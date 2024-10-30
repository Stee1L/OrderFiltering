using System.ComponentModel.DataAnnotations;

namespace OrderFiltering.ViewModels;

public class OrderViewModel
{
    [Required] 
    public float OrderWeight { get; set; }

    [Required] 
    public Guid DistrictId { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
}