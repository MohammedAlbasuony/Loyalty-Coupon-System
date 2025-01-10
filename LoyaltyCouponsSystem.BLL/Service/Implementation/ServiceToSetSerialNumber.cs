using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class ServiceToSetSerialNumber
    {

        private string SserialNumber;

        public long GetSerialNumber(long serialNumber)
        {
            int year = DateTime.Now.Year;
            SserialNumber = Convert.ToString(year) + Convert.ToString(serialNumber);
            return long.Parse(SserialNumber);


        }
    }
}
