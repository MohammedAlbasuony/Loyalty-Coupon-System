using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GeneratePDF;
using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LoyaltyCouponsSystem.BLL.ViewModel.QRCode;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using LoyaltyCouponsSystem.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Diagnostics;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class GenerateQRCodeController : Controller
    {
        private readonly ILogger<GenerateQRCodeController> _logger;
        private readonly IQRCodeGeneratorHelper _QRCodeGeneratorHelper;
        private readonly ApplicationDbContext _context;


        public GenerateQRCodeController(ILogger<GenerateQRCodeController> logger, IQRCodeGeneratorHelper QRCodeGeneratorHelper, ApplicationDbContext context)
        {
            _logger = logger;
            _QRCodeGeneratorHelper = QRCodeGeneratorHelper;
            _context = context;
        }
      
      


        public IActionResult GetAreas(int governorateId)
        {
            var Areas = _context.Areas

                                    .Where(d => d.GovernateId == governorateId)
                                    .Select(d => new { d.Id, d.Name })
                                    .ToList();
            return Ok(Areas);
        }

        public IActionResult GetAllCupones()
        {

            var Result = _context.Coupons.ToList();
            return View(Result);
        }

      

        [HttpGet]
        public IActionResult GenerateCouponsPDF(
    List<(byte[], string)> couponImages,
    int couponsPerRow,
    float horizontalSpacing,
    float verticalSpacing,
    float couponSize
    )
        {
            if (couponImages == null || !couponImages.Any())
            {
                return BadRequest("No coupon images provided.");
            }

            try
            {
                clsGeneratePdfWithCupones generatePdfWithCupones = new clsGeneratePdfWithCupones();



                byte[] pdfData = generatePdfWithCupones.GeneratePDFWithCoupons(couponImages, couponsPerRow, horizontalSpacing, verticalSpacing, couponSize);

                return File(pdfData, "application/pdf", "Coupons.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [HttpGet]
        public IActionResult GetMaxSerialNumber()
        {
            ServiceToMangeCounters serviceToMangeCounters = new ServiceToMangeCounters(_context);

            var MaxSerialNumber = serviceToMangeCounters.GetNextSerialNumInYear();
           

            return Json(MaxSerialNumber);
        }

        public IActionResult DetailsOfCoupones()
        
        
        
        {
            var ViewModel = new QRCodeDetailsViewModel
            {
                governorates = _context.Governorates.ToList()


            };
            return View(ViewModel);
        }


        [HttpPost]
        public IActionResult DetailsOfCoupones(QRCodeDetailsViewModel DetailsVM)
        {

            if (DetailsVM == null)
            {
                return BadRequest("Details are missing.");
            }

            //List Of QRCodes Model
            List<(Coupon, string)> QRCodesList = new List<(Coupon, string)>();
            
            ServiceToMangeCounters serviceToMangeCounters = new ServiceToMangeCounters(_context);

            int currentYear = DateTime.Now.Year;

            //Mapping 

            for (int i = 0; i < DetailsVM.Count; i++)
            {
                Coupon CouponDetails = new Coupon
                {
                    TypeOfCoupone = DetailsVM.TypeOfCoupon,
                    
                    Value = DetailsVM.Value,
                    GovernorateId = DetailsVM.GovernorateId,
                    AreaId = DetailsVM.AreaId,
                    NumInYear = serviceToMangeCounters.GetNextNumInYear(),
                    SerialNumber = DetailsVM.SerialNumber + i

                };

                
                _context.Coupons.Add(CouponDetails);
                _context.SaveChanges();
                QRCodesList.Add((CouponDetails, (Convert.ToString(currentYear) + Convert.ToString(DetailsVM.SerialNumber + i))));

            }
            serviceToMangeCounters.UpdateMaxSerialNum(DetailsVM.Count);



           // Build Base URL
           GenerateListOfCoupons generateListOfCoupons = new GenerateListOfCoupons();

            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            List<(byte[], string)> QRCodesAsBytes = generateListOfCoupons.GenerateQRCodes(QRCodesList, baseUrl);


            //var CodesAsPDF= GenerateCouponsPDF(QRCodesAsBytes,DetailsVM.CouponsPerRow, DetailsVM.HorizontalSpacing, DetailsVM.VerticalSpacing, DetailsVM.CouponWidth, DetailsVM.CouponHeight);


            return GenerateCouponsPDF(QRCodesAsBytes, DetailsVM.CouponsPerRow, DetailsVM.HorizontalSpacing, DetailsVM.VerticalSpacing, DetailsVM.CouponSize);
            
        }


        public IActionResult Index()
        {
            return View();
        }
        // Endpoint To Can Trake QR 
        //[HttpGet("track")]
        //public IActionResult Track(string ID)
        //{
        //    var scanLog = new QRScanLog
        //    {
        //        QR_ID = ID,
        //        ScanTime = DateTime.UtcNow,
        //        UserIP = HttpContext.Connection.RemoteIpAddress.ToString(),
        //        UserAgent = Request.Headers["User-Agent"].ToString(),
        //        NumberOfScans = _context.QRScanLogs.Count(q => q.QR_ID == ID) + 1 //we want to handel this ==>done

        //    };

        //    _context.QRScanLogs.Add(scanLog);
        //    _context.SaveChanges();

        //    return Redirect("https://example.com"); //Redirect To What You Need
        //}

        //// Show Stats
        //public IActionResult Stats()
        //{
        //    var logs = _context.QRScanLogs.ToList();
        //    return View(logs);
        //}


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
