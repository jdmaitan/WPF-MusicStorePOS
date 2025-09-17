namespace TPVWPF.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; } 

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
