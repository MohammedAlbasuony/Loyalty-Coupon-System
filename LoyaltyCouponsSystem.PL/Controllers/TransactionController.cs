using LoyaltyCouponsSystem.DAL.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all transactions with unique ExchangePermission, sorted by the latest added first
        public async Task<IActionResult> AllTransactions()
        {
            // Fetch data from the database
            var transactions = await _context.Transactions
                .Include(t => t.Customer)
                .Include(t => t.Distributor)
                .ToListAsync();

            // Process grouping and ordering in memory
            var distinctTransactions = transactions
                .GroupBy(t => t.ExchangePermission) // Group by ExchangePermission
                .Select(g => g.OrderByDescending(t => t.Timestamp).First()) // Select the latest transaction in each group
                .OrderByDescending(t => t.Timestamp) // Sort the result by Timestamp
                .ToList();

            return View(distinctTransactions);
        }


        // Get coupon details for a specific Exchange Permission
        [HttpGet]
        public async Task<IActionResult> GetCoupons(string exchangePermission)
        {
            var coupons = await _context.Transactions
                .Where(t => t.ExchangePermission == exchangePermission)
                .Select(t => new
                {
                    t.CouponSort,
                    t.CouponType,
                    t.SequenceStart,
                    t.SequenceEnd
                })
                .ToListAsync();

            // Group by sequence range to avoid duplicate cards
            var groupedCoupons = coupons
                .GroupBy(c => new { c.SequenceStart, c.SequenceEnd })
                .Select(g => new
                {
                    SequenceStart = g.Key.SequenceStart,
                    SequenceEnd = g.Key.SequenceEnd,
                    CouponSort = g.First().CouponSort,
                    CouponType = g.First().CouponType
                })
                .ToList();

            return Json(groupedCoupons);
        }
    }
}
