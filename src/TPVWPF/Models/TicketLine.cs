namespace TPVWPF.Models
{
    public class TicketLine
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual Product Product { get; set; }
    }
}
