using System.ComponentModel.DataAnnotations;

namespace ExpirationTracker.Models;

public class Truck
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Truck #")]
    [StringLength(20)]
    public string TruckNo { get; set; } = string.Empty;

    [Display(Name = "Make")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a make.")]
    public int TruckMakeId { get; set; }

    public TruckMake? TruckMake { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [Display(Name = "Year")]
    [StringLength(4)]
    public string? ModelYear { get; set; }

    [Display(Name = "VIN")]
    [StringLength(50)]
    public string? Vin { get; set; }

    [Display(Name = "Driver")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a driver.")]
    public int DriverId { get; set; }

    public Driver? Driver { get; set; }

    [Display(Name = "DOT Expiration")]
    [DataType(DataType.Date)]
    public DateTime? DotInspectionExpiry { get; set; }

    [Display(Name = "Truck Tag Expiration")]
    [DataType(DataType.Date)]
    public DateTime? TruckTagExpiry { get; set; }

    [Display(Name = "IRP Expiration")]
    [DataType(DataType.Date)]
    public DateTime? IrpExpiry { get; set; }

    public ICollection<TruckDocument> Documents { get; set; } = new List<TruckDocument>();
}
