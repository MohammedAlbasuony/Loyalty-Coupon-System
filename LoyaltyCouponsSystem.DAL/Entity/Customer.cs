namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactDetails { get; set; }
        public string Code { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
