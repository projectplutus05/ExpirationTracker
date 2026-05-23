using System.ComponentModel.DataAnnotations;

namespace ExpirationTracker.Models;

public class Driver
{
    public int Id { get; set; }

    [Display(Name = "Driver #")]
    [StringLength(20)]
    public string? DriverNumber { get; set; }

    [Required]
    [Display(Name = "Driver Name")]
    [StringLength(100)]
    public string DriverName { get; set; } = string.Empty;

    [Display(Name = "Physical Expiration")]
    [DataType(DataType.Date)]
    public DateTime? PhysicalExpiry { get; set; }

    [Display(Name = "License Expiration")]
    [DataType(DataType.Date)]
    public DateTime? LicenseExpiry { get; set; }

    public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
}
