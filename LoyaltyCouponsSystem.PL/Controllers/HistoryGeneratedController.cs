
using LoyaltyCouponsSystem.BLL.ViewModel.QRCode;
using LoyaltyCouponsSystem.DAL.DB;
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
      
       

        public async Task< IActionResult> Index()
        {
           

            return View();
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
                    CreationDateTime = c.CreationDateTime
                })
                .ToList();

            // تمرير بيانات الصفحة
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(paginatedCoupons);
        }


    }
}
