using System.ComponentModel.DataAnnotations;

namespace ExpirationTracker.Models;

public class TruckDocument
{
    public int Id { get; set; }

    public int TruckId { get; set; }

    public Truck? Truck { get; set; }

    [Required]
    [StringLength(30)]
    public string ExpirationType { get; set; } = string.Empty;

    [Required]
    [StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
