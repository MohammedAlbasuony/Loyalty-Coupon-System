using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.Entity
{
     public class QRScanLog
    {
        public int Id { get; set; }
        public string QR_ID { get; set; }
        public DateTime ScanTime { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }

        public int NumberOfScans { get; set; }
    }
}
