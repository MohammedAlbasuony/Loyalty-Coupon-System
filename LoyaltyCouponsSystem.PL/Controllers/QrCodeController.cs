using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace QrCodeScannerApp.Controllers
{
    public class QrCodeController : Controller
    {
        // In-memory list to store scanned QR codes (replace with a database in production)
        private static List<string> _scannedQrCodes = new List<string>();

        // GET: /QrCode/Index
        public IActionResult Index()
        {
            // Pass the list of scanned QR codes to the view
            ViewBag.ScannedQrCodes = _scannedQrCodes;
            ViewBag.Counter=_scannedQrCodes.Count;  
            return View();
        }

        // POST: /QrCode/SaveData
        [HttpPost]
        public IActionResult SaveData([FromBody] string qrCodeData)
        {
            if (!string.IsNullOrEmpty(qrCodeData))
            {
                // Save the scanned QR code data
                _scannedQrCodes.Add(qrCodeData);
                return Json(new { success = true, message = "QR code data saved successfully." });
            }

            return Json(new { success = false, message = "Invalid QR code data." });
        }
    }
}