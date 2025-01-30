using LoyaltyCouponsSystem.BLL.Service.Implementation;
using LoyaltyCouponsSystem.BLL.ViewModel.DeliverFormRepToCoust;
using LoyaltyCouponsSystem.BLL.ViewModel.QRCode;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class DilverFromRepToCoustController : Controller
    {

        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DilverFromRepToCoustController(ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           IHttpContextAccessor httpContextAccessor
            )
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task< IActionResult> Index()
        {
            var ModelVM = new DeliverFromRepToCoustVM
            {
                governorates = await _context.Governorates.ToListAsync()

            };



            
            ViewBag.customer = _context.Customers.ToList();
            ViewBag.Technicion=_context.Technicians.ToList();
            ViewBag.Representitive=_context.Users.Where(c=>c.Role== "Representative").ToList();


            return View(ModelVM);
            
        }

       
        

        [HttpPost]
        public async Task<IActionResult> Upload( DeliverFromRepToCoustVM model)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);  // Access logged-in user
            var CreatedBy = currentUser?.UserName;
            
            var filePath = "";
           

            if (model.image != null && model.image.Length > 0)
            {
                filePath = Path.Combine("wwwroot/uploads", model.image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.image.CopyToAsync(stream);
                }

               
            }

            var item = new DeliverFromRepToCoust
            {
                
                CostomerCode = model.SelectedCustomerCode,
                TechnitionCode = model.SelectedTechnicianCode,
                RepresintitiveCode=model.SelectedRepresintitiveCode,
                GovernorateId= model.GovernorateId,
                AreaId = model.AreaId,
                ExchangePermission = model.ExchangePermission,
                ImagePath = filePath,

                
                CreatedBy = CreatedBy

            };
           
            _context.DeliverFromRepToCousts.Add(item);
            
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

       

        public IActionResult Transaction(string ExchangePermissionNumber = "", string governorate = "",
            string area = "", int page = 1, int pageSize = 100)
        {

            var query = _context.DeliverFromRepToCousts
                    .Include(c => c.Governorates) // ربط المحافظات
                    .Include(c => c.Areas)        // ربط المناطق
                    .OrderByDescending(c => c.Timestamp) // ترتيب حسب Serial Number
                    .AsQueryable();

            if (!string.IsNullOrEmpty(ExchangePermissionNumber))
            {
                query = query.Where(c => c.ExchangePermission==ExchangePermissionNumber);
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
                .Select(c => new TransactionForRecieptFromRepToCustVM
                {
                    ExchangePermissionNumber = c.ExchangePermission,
                    GovernorateName = c.GovernorateId != null ? c.Governorates.Name : "N/A",
                    
                    AreaName = c.AreaId != null ? c.Areas.Name : "N/A",
                    
                    CreationDateTime = c.Timestamp,
                    GeneratedBy = c.CreatedBy,
                    CustomerCode=c.CostomerCode,
                    TechnitionCode = c.TechnitionCode,
                    ReprsentitiveCode = c.RepresintitiveCode
                    
                })

                .ToList().OrderByDescending(c => c.CreationDateTime);


            // تمرير بيانات الصفحة
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Governorates = _context.Governorates.Select(s=>s.Name).ToList();

            return View(paginatedCoupons);


            
        }

       
        

        [HttpGet]
        public IActionResult GetAreas(int governorateId)
        {
            var areas = _context.Areas.Where(a => a.GovernateId == governorateId).ToList();
            return Json(areas);
        }


    }
}
