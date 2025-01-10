using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.PL.Models;
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


    }
}
