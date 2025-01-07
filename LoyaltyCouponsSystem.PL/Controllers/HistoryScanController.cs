using LoyaltyCouponsSystem.BLL.Service.Implementation.GenerateQR;
using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    [Route("HistoryScan")]
    public class HistoryScanController : Controller
    {
        private readonly ILogger<HistoryScanController> _logger;
        //private readonly QRCodeGeneratorHelper _QRCodeGeneratorHelper;
        private readonly ApplicationDbContext _context;

        public HistoryScanController(ApplicationDbContext context , ILogger<HistoryScanController> logger)
        {

            _context = context;
            _logger = logger;

        }


        // Endpoint To Can Trake QR 
        [HttpGet("track")]
        public IActionResult Track(string ID)
        {
            var scanLog = new QRScanLog
            {
                QR_ID = ID,
                ScanTime = DateTime.UtcNow,
                UserIP = HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                NumberOfScans = _context.QRScanLogs.Count(q => q.QR_ID == ID) + 1 //we want to handel this ==>done

            };

            _context.QRScanLogs.Add(scanLog);
            _context.SaveChanges();

            return Redirect("https://example.com"); //Redirect To What You Need
        }

        // Show Stats
        [HttpGet("HistoryScans")]
        public IActionResult HistoryScans()
        {
            var logs = _context.QRScanLogs.ToList();
            return View(logs);
        }



    }
}
