using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.DAL.Entity;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR
{
    public class QRCodeGeneratorHelper : IQRCodeGeneratorHelper
    {
        public async Task<byte[]> GenerateQRCodeAsync(Coupon Details, string baseUrl)
        {
            // Build TrackUrl ==> This Url which open when scan QRCode
            string trackUrl = $"{baseUrl}/HistoryScan/track?ID={Details.CouponeId}";

            //start to build QR code
            byte[] QRCode = new byte[0];

            if (!string.IsNullOrEmpty(trackUrl))
            {
                // Generate QRCode contain only on Link For WepPage
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData data = qRCodeGenerator.CreateQrCode(trackUrl, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode bitmap = new BitmapByteQRCode(data);

                // استخدام await لتحويل العملية إلى غير متزامنة (إذا كانت العملية تستغرق وقتًا)
                QRCode = await Task.Run(() => bitmap.GetGraphic(20));
            }

            return QRCode;
        }
    }
}
