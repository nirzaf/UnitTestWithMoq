using System;
using System.Collections.Generic;

using Microsoft.Data.SqlClient;
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
    }
}