using Bogus;

using Microsoft.EntityFrameworkCore;

namespace Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");

        var personList = new Faker<Person>()
            .RuleFor(p => p.PersonID, f => Guid.NewGuid())
            .RuleFor(p => p.PersonName, f => f.Name.FirstName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(100))
            .RuleFor(p => p.CountryID, f => Guid.NewGuid())
            .RuleFor(p => p.Address, f => f.Address.FullAddress())
            .RuleFor(p => p.ReceiveNewsLetters, f => f.Random.Bool())
            .RuleFor(p => p.TIN, f => f.Random.String2(8, "0123456789"))
            .Generate(1000);

        modelBuilder.Entity<Person>().HasData(personList);

    }

    //Generate fake data for testing using Bogus nuget package

    /*
     public class Person
{
  [Key]
  public Guid PersonID { get; set; }

  [StringLength(40)] //nvarchar(40)
  //[Required]
  public string? PersonName { get; set; }

  [StringLength(40)] //nvarchar(40)
  public string? Email { get; set; }

  public DateTime? DateOfBirth { get; set; }

  [StringLength(10)] //nvarchar(100)
  public string? Gender { get; set; }

  //uniqueidentifier
  public Guid? CountryID { get; set; }

  [StringLength(200)] //nvarchar(200)
  public string? Address { get; set; }

  //bit
  public bool ReceiveNewsLetters { get; set; }

  //[Column("TaxIdentificationNumber", TypeName = "varchar(8)")]
  public string? TIN { get; set; }


  [ForeignKey("CountryID")]
  public virtual Country? Country { get; set; }
}
     */

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity traceable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        traceable.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        traceable.ModifiedDate = DateTime.UtcNow;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity traceable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        traceable.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        traceable.ModifiedDate = DateTime.UtcNow;
                        break;
                }
            }
        }

        return base.SaveChanges();
    }
}

public class BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}