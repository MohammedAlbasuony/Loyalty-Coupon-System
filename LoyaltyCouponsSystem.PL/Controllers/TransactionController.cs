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
            var transactions = await _context.Transactions
                .Include(t => t.Customer)
                .Include(t => t.Technician)
                .ToListAsync();

            var distinctTransactions = transactions
                .GroupBy(a => new
                {
                    a.ExchangePermission,
                    a.SequenceStart, // These are strings
                    a.SequenceEnd,   // These are strings
                    a.TechnicianID,
                    a.CustomerID,
                    a.CouponSort,
                    a.CouponType
                })
                .Select(g => g.First())
                .ToList();

            return View(distinctTransactions);



        }

    }
}
