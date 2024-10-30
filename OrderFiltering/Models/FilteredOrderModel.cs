using System.ComponentModel.DataAnnotations;

namespace OrderFiltering.Models;

public class FilteredOrderModel
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    public float OrderWeight { get; set; }
    
    [Required]
    public Guid DistrictId { get; set; }
    
    [Required]
    public string DisctictName { get; set; }
    
    public DateTime DeliveryTime { get; set; }
}