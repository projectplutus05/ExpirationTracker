using System.ComponentModel.DataAnnotations;

namespace ExpirationTracker.Models;

public class TruckMake
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
}