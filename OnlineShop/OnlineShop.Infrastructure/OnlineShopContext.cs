using Microsoft.EntityFrameworkCore;
using OnlineShop.Infrastructure.Models;

namespace OnlineShop.Infrastructure
{
    // Defines the relational schema and the relationships between shop entities.
    public class OnlineShopContext : DbContext
    {
        public OnlineShopContext()
        {
        }

        public OnlineShopContext(DbContextOptions<OnlineShopContext> options)
            : base(options)
        {
        }

        public DbSet<ProductModel> Products => Set<ProductModel>();
        public DbSet<PhoneModel> Phones => Set<PhoneModel>();
        public DbSet<LaptopModel> Laptops => Set<LaptopModel>();
        public DbSet<ProductWarrantyModel> ProductWarranties => Set<ProductWarrantyModel>();
        public DbSet<CustomerModel> Customers => Set<CustomerModel>();
        public DbSet<OrderModel> Orders => Set<OrderModel>();
        public DbSet<OrderItemModel> OrderItems => Set<OrderItemModel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(CreateDefaultConnectionString());
            }
        }

        // Finds the checked-in SQLite file when launched from the solution or output directory.
        internal static string CreateDefaultConnectionString()
        {
            string[] startDirectories = { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };
            foreach (string startDirectory in startDirectories)
            {
                DirectoryInfo? current = new DirectoryInfo(startDirectory);
                while (current is not null)
                {
                    if (File.Exists(Path.Combine(current.FullName, "OnlineShop.slnx")))
                    {
                        return $"Data Source={Path.Combine(current.FullName, "online-shop-lab3.db")}";
                    }

                    current = current.Parent;
                }
            }

            return $"Data Source={Path.Combine(AppContext.BaseDirectory, "online-shop-lab3.db")}";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.UseTptMappingStrategy();
                entity.ToTable("Products");
                entity.HasKey(product => product.Id);
                entity.Property(product => product.Name).HasMaxLength(200).IsRequired();
                entity.Property(product => product.Price).IsRequired();
            });

            modelBuilder.Entity<PhoneModel>(entity =>
            {
                entity.ToTable("Phones");
                entity.Property(phone => phone.Brand).HasMaxLength(100).IsRequired();
                entity.Property(phone => phone.OperatingSystem).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<LaptopModel>(entity =>
            {
                entity.ToTable("Laptops");
                entity.Property(laptop => laptop.Brand).HasMaxLength(100).IsRequired();
                entity.Property(laptop => laptop.Processor).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<ProductWarrantyModel>(entity =>
            {
                entity.ToTable("ProductWarranties");
                entity.HasKey(warranty => warranty.ProductId);
                entity.Property(warranty => warranty.ProductId).ValueGeneratedNever();
                entity.Property(warranty => warranty.Provider).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<ProductModel>()
                .HasOne(product => product.Warranty)
                .WithOne(warranty => warranty.Product)
                .HasForeignKey<ProductWarrantyModel>(warranty => warranty.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(customer => customer.Id);
                entity.Property(customer => customer.Name).HasMaxLength(150).IsRequired();
                entity.Property(customer => customer.Email).HasMaxLength(254).IsRequired();
            });

            modelBuilder.Entity<OrderModel>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(order => order.Id);
            });

            modelBuilder.Entity<OrderItemModel>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(item => item.Id);
            });

            modelBuilder.Entity<CustomerModel>()
                .HasMany(customer => customer.Orders)
                .WithOne(order => order.Customer)
                .HasForeignKey(order => order.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderModel>()
                .HasMany(order => order.Items)
                .WithOne(item => item.Order)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItemModel>()
                .HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
