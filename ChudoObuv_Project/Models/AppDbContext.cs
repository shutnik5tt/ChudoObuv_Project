using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<Manufacturer> Manufacturers { get; set; } = null!;

        public virtual DbSet<Order> Orders { get; set; } = null!;

        public virtual DbSet<Product> Products { get; set; } = null!;

        public virtual DbSet<Role> Roles { get; set; } = null!;

        public virtual DbSet<Size> Sizes { get; set; } = null!;

        public virtual DbSet<StockItem> StockItems { get; set; } = null!;

        public virtual DbSet<Subcategory> Subcategories { get; set; } = null!;

        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ChudoObuv;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}
