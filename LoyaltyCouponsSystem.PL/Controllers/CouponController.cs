using DocumentFormat.OpenXml.InkML;
using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // تأكد من وجود هذه المكتبة

namespace LoyaltyCouponsSystem.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CouponController> _logger;

        public CouponController(ILogger<CouponController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult GetCoupon()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            // جلب البيانات كـ IQueryable
            var couponsQuery = _context.Coupons.AsQueryable();

            // تصفية البيانات بناءً على قيمة البحث
            if (!string.IsNullOrEmpty(searchValue))
            {
                couponsQuery = couponsQuery.Where(c => EF.Functions.Like(c.SerialNumber.ToString(), $"%{searchValue}%") ||
                                                       EF.Functions.Like(c.TypeOfCoupone, $"%{searchValue}%") ||
                                                       EF.Functions.Like(c.Status, $"%{searchValue}%"));
            }

            // فرز البيانات
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                //try
                //{
                //    couponsQuery = couponsQuery.OrderBy($"{sortColumn} {sortColumnDirection}");
                //}
                //catch
                //{
                //    // تجاهل الفرز إذا كان هناك خطأ
                //}
            }

            // إجمالي عدد السجلات بعد التصفية
            var recordsTotal = couponsQuery.Count();

            // تقطيع البيانات حسب الصفحة
            var data = couponsQuery.Skip(skip).Take(pageSize).ToList(); // يتم تحويل البيانات إلى List هنا

            // إعداد استجابة JSON
            var jsonData = new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            };

            return Ok(jsonData);
        }


    }
}



   

