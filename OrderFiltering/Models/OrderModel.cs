using System.ComponentModel.DataAnnotations;

namespace OrderFiltering.Models;

public class OrderModel
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public float OrderWeight { get; set; }
    
    [Required]
    public Guid DistrictId { get; set; }
    
    [Required]
    public DateTime DeliveryTime { get; set; }
}