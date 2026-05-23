using Microsoft.EntityFrameworkCore;
using ExpirationTracker.Models;

namespace ExpirationTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverDocument> DriverDocuments { get; set; }
    public DbSet<Truck> Trucks { get; set; }
    public DbSet<TruckDocument> TruckDocuments { get; set; }
    public DbSet<TruckMake> TruckMakes { get; set; }
    public DbSet<Trailer> Trailers { get; set; }
    public DbSet<TrailerDocument> TrailerDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Truck>()
            .HasOne(t => t.Driver)
            .WithMany(d => d.Trucks)
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DriverDocument>()
            .HasOne(d => d.Driver)
            .WithMany(d => d.Documents)
            .HasForeignKey(d => d.DriverId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DriverDocument>()
            .HasIndex(d => new { d.DriverId, d.ExpirationType })
            .IsUnique();

        modelBuilder.Entity<TrailerDocument>()
            .HasOne(d => d.Trailer)
            .WithMany(t => t.Documents)
            .HasForeignKey(d => d.TrailerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TrailerDocument>()
            .HasIndex(d => new { d.TrailerId, d.ExpirationType })
            .IsUnique();

        modelBuilder.Entity<TruckDocument>()
            .HasOne(d => d.Truck)
            .WithMany(t => t.Documents)
            .HasForeignKey(d => d.TruckId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TruckDocument>()
            .HasIndex(d => new { d.TruckId, d.ExpirationType })
            .IsUnique();

        modelBuilder.Entity<Truck>()
            .HasOne(t => t.TruckMake)
            .WithMany(m => m.Trucks)
            .HasForeignKey(t => t.TruckMakeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TruckMake>().HasData(
            new TruckMake { Id = 1, Name = "Freightliner" },
            new TruckMake { Id = 2, Name = "Peterbilt" },
            new TruckMake { Id = 3, Name = "Volvo" }
        );

        modelBuilder.Entity<Driver>().HasData(
            new Driver { Id = 1, DriverNumber = "0815", DriverName = "Buck", PhysicalExpiry = new DateTime(2026, 8, 31) },
            new Driver { Id = 2, DriverNumber = "7065", DriverName = "Darron" },
            new Driver { Id = 3, DriverNumber = "1190", DriverName = "Jerome", PhysicalExpiry = new DateTime(2027, 1, 31) },
            new Driver { Id = 4, DriverNumber = "1960", DriverName = "Chris G", PhysicalExpiry = new DateTime(2026, 8, 31) },
            new Driver { Id = 5, DriverNumber = "4366", DriverName = "Jeff", PhysicalExpiry = new DateTime(2027, 1, 31) },
            new Driver { Id = 6, DriverNumber = "9271", DriverName = "Q", PhysicalExpiry = new DateTime(2026, 10, 31) },
            new Driver { Id = 7, DriverNumber = "4057", DriverName = "Freddy", PhysicalExpiry = new DateTime(2026, 10, 31) }
        );

        modelBuilder.Entity<Truck>().HasData(
            new Truck { Id = 1, TruckNo = "5800", TruckMakeId = 1, DriverId = 1, DotInspectionExpiry = new DateTime(2026, 11, 2), TruckTagExpiry = new DateTime(2026, 8, 31), IrpExpiry = new DateTime(2026, 8, 31) },
            new Truck { Id = 2, TruckNo = "5003", TruckMakeId = 1, DriverId = 2, DotInspectionExpiry = new DateTime(2027, 1, 16) },
            new Truck { Id = 3, TruckNo = "3439", TruckMakeId = 1, DriverId = 3, DotInspectionExpiry = new DateTime(2026, 9, 9), TruckTagExpiry = new DateTime(2027, 1, 31), IrpExpiry = new DateTime(2027, 1, 31) },
            new Truck { Id = 4, TruckNo = "3440", TruckMakeId = 1, DriverId = 3, DotInspectionExpiry = new DateTime(2027, 2, 26), TruckTagExpiry = new DateTime(2027, 1, 31), IrpExpiry = new DateTime(2027, 1, 31) },
            new Truck { Id = 5, TruckNo = "5737", TruckMakeId = 1, DriverId = 4, DotInspectionExpiry = new DateTime(2026, 11, 2), TruckTagExpiry = new DateTime(2026, 8, 31), IrpExpiry = new DateTime(2026, 8, 31) },
            new Truck { Id = 6, TruckNo = "5736", TruckMakeId = 1, DriverId = 4, DotInspectionExpiry = new DateTime(2026, 11, 9), TruckTagExpiry = new DateTime(2026, 8, 31), IrpExpiry = new DateTime(2026, 8, 31) },
            new Truck { Id = 7, TruckNo = "6195", TruckMakeId = 1, DriverId = 5, DotInspectionExpiry = new DateTime(2026, 8, 19), TruckTagExpiry = new DateTime(2027, 2, 28), IrpExpiry = new DateTime(2027, 1, 31) },
            new Truck { Id = 8, TruckNo = "6181", TruckMakeId = 1, DriverId = 6, DotInspectionExpiry = new DateTime(2026, 8, 6), TruckTagExpiry = new DateTime(2026, 10, 31), IrpExpiry = new DateTime(2026, 10, 31) },
            new Truck { Id = 9, TruckNo = "455", TruckMakeId = 1, DriverId = 7, DotInspectionExpiry = new DateTime(2027, 3, 16), TruckTagExpiry = new DateTime(2026, 10, 31), IrpExpiry = new DateTime(2026, 10, 31) }
        );

        modelBuilder.Entity<Trailer>().HasData(
            new Trailer { Id = 1, AssignedTo = "Buck", TrailerNumber = "7432", HasHeadboard = false, Year = "2020", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482CXL3196914", TagNumber = "1A0BUM2", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2027, 3, 8) },
            new Trailer { Id = 2, AssignedTo = "Darron", TrailerNumber = "5863", HasHeadboard = true, Year = "2020", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482C3L3196866", TagNumber = "1A0BUHX", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2027, 3, 17) },
            new Trailer { Id = 3, AssignedTo = "Jerome", TrailerNumber = "5860", HasHeadboard = false, Year = "2018", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482CXJ3060408", TagNumber = "1A0BUM1", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2027, 1, 6) },
            new Trailer { Id = 4, AssignedTo = "Chris G", TrailerNumber = "943", HasHeadboard = false, Year = "2015", Make = "Transcraft", Model = "48' Flatbed", Vin = "1TTF482C3F3895967", TagNumber = "62AOMBH", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2026, 11, 2) },
            new Trailer { Id = 5, AssignedTo = "Jeff", TrailerNumber = "5862", HasHeadboard = false, Year = "2019", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482C0K3142116", TagNumber = "1AOBUHY", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2026, 7, 9) },
            new Trailer { Id = 6, AssignedTo = "Q", TrailerNumber = "5854", HasHeadboard = true, Year = "2017", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482C1H3027890", TagNumber = "A004585", TagNotes = "Perm", AnnualInspExpiry = new DateTime(2027, 3, 14) },
            new Trailer { Id = 7, AssignedTo = "Freddy", TrailerNumber = "7201", HasHeadboard = true, Year = "2012", Make = "Transcraft", Model = "48' Flatbed", Vin = "1TTF482C5C3576713", TagNumber = "1A0BUHE", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2027, 3, 14) },
            new Trailer { Id = 8, AssignedTo = "Rented", TrailerNumber = "5861", HasHeadboard = false, Year = "2019", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482C4K3133001", TagNumber = "1A0BUM0", TagExpiry = new DateTime(2026, 11, 30), AnnualInspExpiry = new DateTime(2026, 10, 23) },
            new Trailer { Id = 9, AssignedTo = null, TrailerNumber = "60852", HasHeadboard = false, Year = "2015", Make = "Transcraft", Model = "554C Eagle II", Vin = "1TTF482C2F3873328", TagNumber = "A138145", TagNotes = "Perm", AnnualInspExpiry = new DateTime(2027, 1, 19) }
        );
    }
}
