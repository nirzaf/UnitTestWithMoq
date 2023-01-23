using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace CRUDTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
 protected override void ConfigureWebHost(IWebHostBuilder builder)
 {
  base.ConfigureWebHost(builder);

  builder.UseEnvironment("Test");

  builder.ConfigureServices(services => {
   var descripter = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

   if (descripter != null)
   {
    services.Remove(descripter);
   }
   services.AddDbContext<ApplicationDbContext>(options =>
   {
    options.UseInMemoryDatabase("DatbaseForTesting");
   });
  });
 }
}