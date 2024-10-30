using System.ComponentModel.DataAnnotations;

namespace OrderFiltering.Models;

public class DistrictModel
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
}