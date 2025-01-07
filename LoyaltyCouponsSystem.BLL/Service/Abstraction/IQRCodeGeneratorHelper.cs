using LoyaltyCouponsSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Abstraction
{
    public interface IQRCodeGeneratorHelper
    {
        //This take Details of QrCode like,IdUniqe with guid value,Region,
        //year,number in year description
        //and return qrcode as array of byte  
        //and this QRCode contain on URL this when you Scan will open it 
        //this URL is BaseUrl +Id
        byte[] GenerateQRCode(Coupon Details, string baseUrl);
    }
}
