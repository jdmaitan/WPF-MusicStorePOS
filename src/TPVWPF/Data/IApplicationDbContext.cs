using Microsoft.EntityFrameworkCore;
using TPVWPF.Models;

namespace TPVWPF.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<TicketLine> TicketLines { get; set; }
        DbSet<Ticket> Tickets { get; set; }
        DbSet<User> Users { get; set; }
        public void SaveChanges();
    }
}