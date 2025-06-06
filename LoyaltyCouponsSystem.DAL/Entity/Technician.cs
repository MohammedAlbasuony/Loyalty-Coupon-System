﻿using LoyaltyCouponsSystem.DAL.DB;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Technician
    {
        public int TechnicianID { get; set; }
        public string Name { get; set; }
        public string? NickName { get; set; }
        public string NationalID { get; set; }
        public int PhoneNumber1 { get; set; }
        public int? PhoneNumber2 { get; set; }
        public int? PhoneNumber3 { get; set; }
        public string Governate { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public ICollection<Coupon> Coupons { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Technician()
        {
            TechnicianCustomers = new List<TechnicianCustomer>();
            TechnicianUsers = new List<TechnicianUser>();
        }
        public ICollection<TechnicianCustomer> TechnicianCustomers { get; set; }
        public ICollection<TechnicianUser> TechnicianUsers { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
