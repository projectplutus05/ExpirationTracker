using System.ComponentModel.DataAnnotations;

namespace ExpirationTracker.Models;

public class Trailer
{
    public int Id { get; set; }

    [Display(Name = "Assigned To")]
    [StringLength(100)]
    public string? AssignedTo { get; set; }

    [Required]
    [Display(Name = "Trailer #")]
    [StringLength(20)]
    public string TrailerNumber { get; set; } = string.Empty;

    [Display(Name = "Headboard")]
    public bool HasHeadboard { get; set; }

    [StringLength(10)]
    public string? Year { get; set; }

    [StringLength(100)]
    public string? Make { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [Display(Name = "VIN")]
    [StringLength(50)]
    public string? Vin { get; set; }

    [Display(Name = "Tag #")]
    [StringLength(30)]
    public string? TagNumber { get; set; }

    [Display(Name = "Tag Expiry")]
    [DataType(DataType.Date)]
    public DateTime? TagExpiry { get; set; }

    [Display(Name = "Tag Notes")]
    [StringLength(50)]
    public string? TagNotes { get; set; }

    [Display(Name = "Annual Inspection Expiry")]
    [DataType(DataType.Date)]
    public DateTime? AnnualInspExpiry { get; set; }

    public ICollection<TrailerDocument> Documents { get; set; } = new List<TrailerDocument>();
}
