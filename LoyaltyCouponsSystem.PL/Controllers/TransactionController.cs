using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AllTransactions()
        {
            var transactions = (await _context.Transactions
                .Include(t => t.Customer) 
                .Include(t => t.Technician)
                .ToListAsync()).DistinctBy(a => new {a.ExchangePermission,});

            return View(transactions);
        }

    }
}
