namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Technician
    {
        public int TechnicianID { get; set; }
        public string Name { get; set; }
        public string ContactDetails { get; set; }
        public string Code { get; set; }
        public ICollection<Coupon> Coupons { get; set; }
    }
}
