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

        List<Guid> generatedCountryIds = new List<Guid>();
        
        var countries = new Faker<Country>()
            .RuleFor(c => c.CountryID, f => {
                var countryId = f.Random.Guid();
                generatedCountryIds.Add(countryId);
                return countryId;
            })
            .RuleFor(c => c.CountryName, f => f.Address.Country())
            .Generate(100);

        
        modelBuilder.Entity<Country>().HasData(countries);

        var personList = new Faker<Person>()
            .RuleFor(p => p.PersonID, f => Guid.NewGuid())
            .RuleFor(p => p.PersonName, f => f.Name.FirstName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Past(100))
            .RuleFor(p => p.CountryID, f => f.PickRandom(generatedCountryIds))
            .RuleFor(p => p.Address, f => f.Address.FullAddress())
            .RuleFor(p => p.ReceiveNewsLetters, f => f.Random.Bool())
            .RuleFor(p => p.TIN, f => f.Finance.Iban())
            .Generate(1000);

        modelBuilder.Entity<Person>().HasData(personList);
    }

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