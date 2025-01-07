using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation.GeneratePDF
{

    public class clsGeneratePdfWithCupones
    {
        

            public byte[] GeneratePDFWithCoupons(
                List<(byte[] ImageBytes, string SerialNumber)> couponData, // تعديل: إضافة SerialNumber لكل كوبون
                int couponsPerRow,
                float horizontalSpacingCm,
                float verticalSpacingCm,
                float couponSize
                )
            {
                // تحويل cm إلى px
                float ConvertCmToPx(float cm) => cm * (96f / 2.54f);

                // تحويل القيم المدخلة إلى px
                float horizontalSpacing = ConvertCmToPx(horizontalSpacingCm);
                float verticalSpacing = ConvertCmToPx(verticalSpacingCm);
                float couponWidth = ConvertCmToPx(couponSize);
                float couponHeight = ConvertCmToPx(couponSize);

                // حجم الصفحة (A4)
                var pageWidth = PageSize.A4.Width; // عرض الصفحة
                var pageHeight = PageSize.A4.Height; // ارتفاع الصفحة

                using MemoryStream ms = new MemoryStream();
                using var document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // الإعداد الأولي لمواضع x و y
                float currentX = horizontalSpacing;
                float currentY = pageHeight - couponHeight - verticalSpacing;

                int couponsInCurrentRow = 0;

                foreach (var (imageBytes, serialNumber) in couponData)
                {
                    // تحميل الصورة
                    var image = Image.GetInstance(new MemoryStream(imageBytes));
                    image.ScaleAbsolute(couponWidth, couponHeight); // حجم الكوبون
                    image.SetAbsolutePosition(currentX, currentY); // موضع الكوبون

                    document.Add(image); // إضافة الصورة إلى الصفحة

                    // إضافة النص (Serial Number) تحت الصورة
                    float textX = currentX;
                    float textY = currentY - 15; // 15px أسفل الصورة (يمكن تغييره حسب الحاجة)
                    var font = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                    ColumnText.ShowTextAligned(
                        writer.DirectContent,
                        Element.ALIGN_CENTER,
                        new Phrase(serialNumber, font),
                        textX + (couponWidth / 2), // مركز الكوبون
                        textY,
                        0 // الزاوية
                    );

                    // تحديث مواضع x و y
                    currentX += couponWidth + horizontalSpacing;
                    couponsInCurrentRow++;

                    // إذا كانت الصف ممتلئة
                    if (couponsInCurrentRow >= couponsPerRow)
                    {
                        currentX = horizontalSpacing; // إعادة الموضع الأفقي
                        currentY -= couponHeight + verticalSpacing; // الانتقال إلى الصف التالي
                        couponsInCurrentRow = 0;
                    }

                    // إذا كانت الصفحة ممتلئة
                    if (currentY <= verticalSpacing)
                    {
                        document.NewPage(); // إضافة صفحة جديدة
                        currentX = horizontalSpacing;
                        currentY = pageHeight - couponHeight - verticalSpacing;
                    }
                }

                document.Close();
                return ms.ToArray(); // إرجاع الـ PDF كـ Array من البايتات
            }



      


    }
}
