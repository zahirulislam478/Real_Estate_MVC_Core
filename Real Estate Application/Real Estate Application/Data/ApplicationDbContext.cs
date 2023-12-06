using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Real_Estate_Application.Models;

namespace Real_Estate_Application.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // create a DbSet CRUD object for each model
        public DbSet<Real_Estate_Application.Models.Property> Properties { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
    }
}