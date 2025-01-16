using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateExcel
{
    public class clsGenerateExcelWithCoupons
    {
        public async Task<byte[]> GenerateExcelWithCouponsAsync(
            List<Coupon> coupons,
            string baseURL,
            float qrCodeSizeCm)
        {
            // تحويل cm إلى px (لضبط حجم QR)
            int ConvertCmToPx(float cm) => (int)(cm * (96f / 2.54f));

            int qrCodeSizePx = ConvertCmToPx(qrCodeSizeCm);

            // إعداد مكتبة EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Coupons");

                // إعداد الرأس (Header)
                worksheet.Cells[1, 1].Value = "QR Code";
                worksheet.Cells[1, 2].Value = "Serial Number";
                worksheet.Row(1).Style.Font.Bold = true;

                int currentRow = 2;

                foreach (var coupon in coupons)
                {
                    // إنشاء QR Code للصورة
                    QRCodeGeneratorHelper qrCodeGeneratorHelper = new QRCodeGeneratorHelper();
                    byte[] qrCode = await qrCodeGeneratorHelper.GenerateQRCodeAsync(coupon, baseURL);

                    using (var ms = new MemoryStream(qrCode))
                    {
                        // إدراج QR Code كصورة في الخلية
                        var picture = worksheet.Drawings.AddPicture($"QRCode_{currentRow}", ms);
                        picture.SetPosition(currentRow - 1, 0, 0, 0); // الصف والعمود
                        picture.SetSize(qrCodeSizePx, qrCodeSizePx); // ضبط الحجم
                    }

                    // إدراج Serial Number
                    worksheet.Cells[currentRow, 2].Value = coupon.SerialNumber;

                    currentRow++;
                }

                // تنسيق الأعمدة
                worksheet.Column(1).Width = 20; // عرض عمود QR Code
                worksheet.Column(2).AutoFit(); // ضبط عرض عمود Serial Number تلقائيًا

                // حفظ الملف في الذاكرة
                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
