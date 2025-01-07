using LoyaltyCouponsSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR
{
    public class GenerateListOfCoupons
    {
        public List<(byte[], string)> GenerateQRCodes(List<(Coupon, string)> coupons, string BaseURL)
        {
            List<(byte[], string)> qrCodes = new List<(byte[], string)>();

            foreach (var coupon in coupons)
            {

                QRCodeGeneratorHelper qRCodeGeneratorHelper = new QRCodeGeneratorHelper();

                byte[] qrCode = qRCodeGeneratorHelper.GenerateQRCode(coupon.Item1, BaseURL);
                qrCodes.Add((qrCode, coupon.Item2));
            }

            return qrCodes;
        }
    }
}
