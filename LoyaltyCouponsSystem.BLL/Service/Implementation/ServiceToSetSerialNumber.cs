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

        private long LongSerialNumber;

        public string GetSerialNumber(string serialNumber,int i)
        {
            int year = DateTime.Now.Year;
            SserialNumber = Convert.ToString(year) + serialNumber;
            LongSerialNumber = long.Parse(SserialNumber) + i;
            return Convert.ToString( LongSerialNumber);


        }
    }
}
