
using Autofac.Core;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.ViewModel.QRCode;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.PL.Controllers
{


    public class HistoryGeneratedController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HistoryGeneratedController> _logger;

        public HistoryGeneratedController(ILogger<HistoryGeneratedController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
      
       



        public async Task< IActionResult> TransactionHistory(
          
    string fromSequence = "",
    string toSequence = "",
    string typeOfCoupon = "",
    string governorate = "",
    string area = "",
    string status = "",
    int page = 1,
    int pageSize = 100)
        {
            ServiceToManageStatues serviceToManageStatues = new ServiceToManageStatues(_context);

            var query = _context.qRCodeTransactionGenerateds
                    .Include(c => c.Governorates) // ربط المحافظات
                    .Include(c => c.Areas)        // ربط المناطق
                    .OrderByDescending(c => c.CreationDateTime) // ترتيب حسب Serial Number
                    .AsQueryable();

            if (!string.IsNullOrEmpty(fromSequence) && !string.IsNullOrEmpty(toSequence))
            {
                query = query.Where(c => c.FromSerialNumber== fromSequence ||
                                         c.ToSerialNumber== toSequence);
            }
            if (!string.IsNullOrEmpty(typeOfCoupon))
            {
                query = query.Where(c => c.TypeOfCoupone.Contains(typeOfCoupon));
            }
            if (!string.IsNullOrEmpty(governorate))
            {
                query = query.Where(c => c.Governorates != null && c.Governorates.Name.Contains(governorate));
            }
            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(c => c.Areas != null && c.Areas.Name.Contains(area));
            }

            // إجمالي عدد العناصر بعد التصفية
            int totalCount = query.Count();

            // تطبيق الترقيم
            var paginatedCoupons = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new HistoryTransactionGeneratedQRVM
                {
                    FromSerialNumber=c.FromSerialNumber,
                    ToSerialNumber=c.ToSerialNumber,
                    TypeOfCoupone = c.TypeOfCoupone,
                    Governorate = c.Governorates != null ? c.Governorates.Name : "N/A",
                    Area = c.Areas != null ? c.Areas.Name : "N/A",
                    Value = c.Value,
                    CreationDateTime = c.CreationDateTime,
                    GeneratedBy = c.GeneratedBy
                    ,NumberOfCoupones=c.NumberOfCoupones,
                    FlagToPrint = serviceToManageStatues.DecideToPrintOrNo(c.FromSerialNumber,c.ToSerialNumber)

                })
                
                .ToList().OrderByDescending(c=>c.CreationDateTime);
            

            // تمرير بيانات الصفحة
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Governorates = GetGovernorates();

            return View(paginatedCoupons);

           
        }

        public IEnumerable<string> GetGovernorates()
        {
            return _context.Governorates.Select(g => g.Name).ToList();
        }

        [HttpGet]
        public JsonResult GetAreas(string governorate)
        {
            if (string.IsNullOrEmpty(governorate))
            {
                return Json(new List<string>());
            }

            var areas = GetAreasByGovernorate(governorate); // Fetch areas based on governorate
            return Json(areas);
        }
        public IEnumerable<string> GetAreasByGovernorate(string governorate)
        {
            return _context.Areas
                .Where(a => a.Governorate.Name == governorate)
                .Select(a => a.Name)
                .ToList();
        }

        [HttpGet]

        public IActionResult GetAllCoupons(
    string serialNumber = "",
    string fromSequence = "",
    string toSequence = "",
    string typeOfCoupon = "",
    string governorate = "",
    string area = "",
    string status = "",
    int page = 1,
    int pageSize = 100)
        {
            // بدء استعلام القسائم
            var query = _context.Coupons
                .Include(c => c.Governorates) // ربط المحافظات
                .Include(c => c.Areas)        // ربط المناطق
                .OrderBy(c => c.SerialNumber) // ترتيب حسب Serial Number
                .AsQueryable();

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(serialNumber))
            {
                query = query.Where(c => c.SerialNumber.Contains(serialNumber));
            }
            if (!string.IsNullOrEmpty(fromSequence) && !string.IsNullOrEmpty(toSequence))
            {
                query = query.Where(c => string.Compare(c.SerialNumber, fromSequence) >= 0 &&
                                         string.Compare(c.SerialNumber, toSequence) <= 0);
            }
            if (!string.IsNullOrEmpty(typeOfCoupon))
            {
              
                query = query.Where(c => c.TypeOfCoupone.Contains(typeOfCoupon));
            }
            if (!string.IsNullOrEmpty(governorate))
            {
                query = query.Where(c => c.Governorates != null && c.Governorates.Name.Contains(governorate));
            }
            if (!string.IsNullOrEmpty(area))
            {
                query = query.Where(c => c.Areas != null && c.Areas.Name.Contains(area));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status.ToString().Contains(status));
            }

            // إجمالي عدد العناصر بعد التصفية
            int totalCount = query.Count();

            // تطبيق الترقيم
            var paginatedCoupons = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CouponViewModel
                {
                    SerialNumber = c.SerialNumber,
                    TypeOfCoupone = c.TypeOfCoupone,
                    GovernorateName = c.Governorates != null ? c.Governorates.Name : "N/A",
                    AreaName = c.Areas != null ? c.Areas.Name : "N/A",
                    Value = c.Value,
                    NumInYear = c.NumInYear,
                    Status = c.Status,
                    CreationDateTime = c.CreationDateTime,
                    CreatedBy=c.CreatedBy,
                   
                    CustomerCode = c.CustomerCode,
                    DistubuterCode=c.DistributorCode,
                    RepresentativeCode = c.RepresentativeCode


                })
                .ToList();

            // تمرير بيانات الصفحة
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Governorates = GetGovernorates();

            return View(paginatedCoupons);
        }


    }
}
