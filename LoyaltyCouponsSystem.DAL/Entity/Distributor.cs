using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Distributor
    {
        public int DistributorID { get; set; }
        public string Name { get; set; }
        public int PhoneNumber1 { get; set; }
        public string? Governate { get; set; }
        public string? City { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; } 
        public ICollection<Customer> Customers { get; set; }
    }
}
