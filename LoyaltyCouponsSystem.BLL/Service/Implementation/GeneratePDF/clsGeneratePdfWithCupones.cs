using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation.GeneratePDF
{
    public class clsGeneratePdfWithCupones
    {
        public async Task<byte[]> GeneratePDFWithCouponsAsync(
            List<(byte[] ImageBytes, string SerialNumber)> couponData,
            int couponsPerRow,
            float horizontalSpacingCm,
            float verticalSpacingCm,
            float couponSize
        )
        {
            // تحويل cm إلى px
            float ConvertCmToPx(float cm) => cm * (96f / 2.54f);

            float horizontalSpacing = ConvertCmToPx(horizontalSpacingCm);
            float verticalSpacing = ConvertCmToPx(verticalSpacingCm);
            float couponWidth = ConvertCmToPx(couponSize);
            float couponHeight = ConvertCmToPx(couponSize);

            var pageWidth = PageSize.A4.Width;
            var pageHeight = PageSize.A4.Height;

            using MemoryStream ms = new MemoryStream();
            using var document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            document.Open();

            float currentX = horizontalSpacing;
            float currentY = pageHeight - couponHeight - verticalSpacing;

            int couponsInCurrentRow = 0;

            foreach (var (imageBytes, serialNumber) in couponData)
            {
                // معالجة الكوبونات بصورة غير متزامنة
                await Task.Run(() =>
                {
                    try
                    {
                        var image = Image.GetInstance(new MemoryStream(imageBytes));
                        image.ScaleAbsolute(couponWidth, couponHeight);
                        image.SetAbsolutePosition(currentX, currentY);
                        document.Add(image);

                        float textX = currentX;
                        float textY = currentY - 15;
                        var font = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                        ColumnText.ShowTextAligned(
                            writer.DirectContent,
                            Element.ALIGN_CENTER,
                            new Phrase(serialNumber, font),
                            textX + (couponWidth / 2),
                            textY,
                            0
                        );

                        currentX += couponWidth + horizontalSpacing;
                        couponsInCurrentRow++;

                        if (couponsInCurrentRow >= couponsPerRow)
                        {
                            currentX = horizontalSpacing;
                            currentY -= couponHeight + verticalSpacing;
                            couponsInCurrentRow = 0;
                        }

                        if (currentY <= verticalSpacing)
                        {
                            document.NewPage();
                            currentX = horizontalSpacing;
                            currentY = pageHeight - couponHeight - verticalSpacing;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing coupon {serialNumber}: {ex.Message}");
                    }
                });
            }

            document.Close();
            return ms.ToArray();
        }
    }
}
