using LoyaltyCouponsSystem.BLL.ViewModel.DeliverFormRepToCoust;
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
        public IActionResult Index()
        {
            var ModelVM= new DeliverFromRepToCoustVM() ;
             ViewBag.Governorates = _context.Governorates.ToList();
            return View(ModelVM);
            
        }

        [HttpPost]
        public IActionResult Search(string query)
        {
            var results = _context.DeliverFromRepToCousts
                .Where(i =>  i.CostomerCode.Contains(query))
                .OrderByDescending(i => EF.Functions.Like(i.CostomerCode, $"{query}%")) // Most match at top
                .ToList();

            return PartialView("_SearchResults", results);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile image, DeliverFromRepToCoustVM model)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);  // Access logged-in user
            var CreatedBy = currentUser?.UserName;
            
            var filePath = "";
           

            if (image != null && image.Length > 0)
            {
                filePath = Path.Combine("wwwroot/uploads", image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

               
            }

            var item = new DeliverFromRepToCoust
            {
                
                CostomerCode = model.SelectedCustomerCode,
                TechnitionCode = model.SelectedTechnicianCode,
                GovernorateName = model.Governorate,
                AreaName = model.Area,
                ExchangePermission = model.ExchangePermission,
                ImagePath = filePath,

                

                Timestamp = DateTime.Now,

                CreatedBy = CreatedBy

            };

            _context.DeliverFromRepToCousts.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetAreas(int governorateId)
        {
            var areas = _context.Areas.Where(a => a.GovernateId == governorateId).ToList();
            return Json(areas);
        }


    }
}
