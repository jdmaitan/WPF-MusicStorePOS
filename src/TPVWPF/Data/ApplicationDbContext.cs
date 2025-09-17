using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TPVWPF.Models;

namespace TPVWPF.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketLine> TicketLines { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local); Database=TPV; User Id=usu1; Password=1234; Integrated Security=False; Persist Security Info=False; TrustServerCertificate=True").LogTo(Console.WriteLine, LogLevel.Information); ;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TicketLine>()
                .HasKey(tl => tl.Id);

            modelBuilder.Entity<TicketLine>()
                .HasOne(tl => tl.Ticket)
                .WithMany(t => t.TicketLines)
                .HasForeignKey(tl => tl.TicketId);

            modelBuilder.Entity<TicketLine>()
                .HasOne(tl => tl.Product)
                .WithMany(p => p.TicketLines)
                .HasForeignKey(tl => tl.ProductId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CustomerId);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Role = UserRole.Admin }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Guitarra", Price = 500.00M, Description = "Guitarra Electrica Ibanez", Category = "Instrumentos" },
                new Product { Id = 2, Name = "Bajo de 5 cuerdas", Price = 500.00M, Description = "Bajo Electrico Fender", Category = "Instrumentos" },
                new Product { Id = 3, Name = "Teclado", Price = 500.00M, Description = "Teclado Roland", Category = "Instrumentos" },
                new Product { Id = 4, Name = "Guitarra acustica yamaha", Price = 123.00M, Description = "Guitarra de 6 cuerdas con cuerdas de nylon", Category = "Instrumentos" },
                new Product { Id = 5, Name = "Flauta transversa", Price = 324.00M, Description = "Flauta orquestal", Category = "Instrumentos" },
                new Product { Id = 6, Name = "Batería Tama", Price = 432.00M, Description = "Batería con doble bombo", Category = "Percusion" },
                new Product { Id = 7, Name = "Pedalera Ibanez TS-808", Price = 80.00M, Description = "Pedalera para distorsión de guitarra eléctrica", Category = "Accesorios" }
            );


            base.OnModelCreating(modelBuilder);
        }

        void IApplicationDbContext.SaveChanges()
        {
            base.SaveChanges();
        }
    }
}