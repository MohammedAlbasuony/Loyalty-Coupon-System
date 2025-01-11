
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


        public IActionResult GetAllCoupons(string serialNumber = "", int page = 1, int pageSize = 100)
        {
            // ربط البيانات مع المحافظات والمناطق
            var query = _context.Coupons
                .Include(c => c.Governorates) // ربط جدول المحافظات
                .Include(c => c.Areas)        // ربط جدول المناطق
                .OrderBy(c => c.SerialNumber) // ترتيب حسب Serial Number

                // إضافة شرط البحث إذا تم توفير Serial Number
                .Where(c => string.IsNullOrEmpty(serialNumber) || c.SerialNumber.ToString().Contains(serialNumber));

            // إجمالي عدد العناصر بعد التصفية (لصفحات الترقيم)
            int totalCount = query.Count();

            // تطبيق التقسيم إلى صفحات
            var paginatedCoupons = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CouponViewModel
                {
                    SerialNumber = c.SerialNumber.ToString(),
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
