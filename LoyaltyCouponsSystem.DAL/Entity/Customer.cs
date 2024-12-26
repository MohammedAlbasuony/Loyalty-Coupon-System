using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class Customer
    { 
            public int CustomerID { get; set; }
            public string Name { get; set; } 
            public string ContactDetails { get; set; } 

            public ICollection<Transaction> Transactions { get; set; }   
    }
}
