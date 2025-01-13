using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
    public class DistributorCustomer
    {
        public int DistributorID { get; set; }
        public Distributor Distributor { get; set; }

        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}
