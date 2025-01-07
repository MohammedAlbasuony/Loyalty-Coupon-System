using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GeneratePDF;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LoyaltyCouponsSystem.BLL.ViewModel.QRCode;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class GenerateQRCodeController : Controller
    {
        private readonly ILogger<GenerateQRCodeController> _logger;
        private readonly IQRCodeGeneratorHelper _QRCodeGeneratorHelper;
        private readonly ApplicationDbContext _context;

        public GenerateQRCodeController(
            ILogger<GenerateQRCodeController> logger,
            IQRCodeGeneratorHelper QRCodeGeneratorHelper,
            ApplicationDbContext context)
        {
            _logger = logger;
            _QRCodeGeneratorHelper = QRCodeGeneratorHelper;
            _context = context;
        }

        public async Task<IActionResult> GetAreas(int governorateId)
        {
            var areas = await _context.Areas
                .Where(d => d.GovernateId == governorateId)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();

            return Ok(areas);
        }

        public async Task<IActionResult> GetAllCupones()
        {
            var result = await _context.Coupons.ToListAsync();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCouponsPDF(
            List<(byte[], string)> couponImages,
            int couponsPerRow,
            float horizontalSpacing,
            float verticalSpacing,
            float couponSize)
        {
            if (couponImages == null || !couponImages.Any())
            {
                return BadRequest("No coupon images provided.");
            }

            try
            {
                var generatePdfWithCupones = new clsGeneratePdfWithCupones();
                byte[] pdfData = await generatePdfWithCupones.GeneratePDFWithCouponsAsync(
                    couponImages, couponsPerRow, horizontalSpacing, verticalSpacing, couponSize);

                return File(pdfData, "application/pdf", "Coupons.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMaxSerialNumber()
        {
            var serviceToMangeCounters = new ServiceToMangeCounters(_context);
            var maxSerialNumber = await serviceToMangeCounters.GetNextSerialNumInYearAsync();

            return Json(maxSerialNumber);
        }

        public async Task<IActionResult> DetailsOfCoupones()
        {
            var viewModel = new QRCodeDetailsViewModel
            {
                governorates = await _context.Governorates.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DetailsOfCoupones(QRCodeDetailsViewModel detailsVM)
        {
            if (detailsVM == null)
            {
                return BadRequest("Details are missing.");
            }

            // قائمة الـ QR Codes
            var qrCodesList = new List<(Coupon, string)>();
            var serviceToMangeCounters = new ServiceToMangeCounters(_context);
            int currentYear = DateTime.Now.Year;

            // إنشاء الكوبونات
            for (int i = 0; i < detailsVM.Count; i++)
            {
                var couponDetails = new Coupon
                {
                    TypeOfCoupone = detailsVM.TypeOfCoupon,
                    Value = detailsVM.Value,
                    GovernorateId = detailsVM.GovernorateId,
                    AreaId = detailsVM.AreaId,
                    NumInYear = await serviceToMangeCounters.GetNextNumInYearAsync(),
                    SerialNumber = detailsVM.SerialNumber + i
                };

                await _context.Coupons.AddAsync(couponDetails);
                await _context.SaveChangesAsync();
                qrCodesList.Add((couponDetails, $"{currentYear}{detailsVM.SerialNumber + i}"));
            }

            await serviceToMangeCounters.UpdateMaxSerialNumAsync(detailsVM.Count);

            // إنشاء QR Codes
            var generateListOfCoupons = new GenerateListOfCoupons();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            var qrCodesAsBytes = generateListOfCoupons.GenerateQRCodesAsync(qrCodesList, baseUrl);

            // إنشاء ملف PDF من QR Codes
            return await GenerateCouponsPDF(await qrCodesAsBytes.ConfigureAwait(false), detailsVM.CouponsPerRow, detailsVM.HorizontalSpacing, detailsVM.VerticalSpacing, detailsVM.CouponSize);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
