namespace TPVWPF.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<TicketLine> TicketLines { get; set; }
    }
}
