using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class TechnicianController : Controller
    {
        private readonly ITechnicianService _technicianService;

        public TechnicianController(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllTechnicians()
        {
            var result = await _technicianService.GetAllAsync();
            return View(result);
        }

        public async Task<IActionResult> GetTechnicianById(string id)
        {
            var result = await _technicianService.GetByIdAsync(id);
            return View(result);
        }

        [HttpGet]
        public IActionResult AddTechnician()
        {
            var technicianViewModel = new TechnicianViewModel
            {
                Governates = new List<SelectListItem>
        {
            new SelectListItem { Text = "Cairo", Value = "Cairo" },
            new SelectListItem { Text = "Gharbeia", Value = "Gharbeia" },
            new SelectListItem { Text = "Sharqeia", Value = "Sharqeia" },
            new SelectListItem { Text = "Alexandria", Value = "Alexandria" }
        }
            };
            return View(technicianViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTechnician(TechnicianViewModel technicianViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _technicianService.AddAsync(technicianViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllTechnicians");
                }
                ModelState.AddModelError("", "Unable to add technician. Please try again.");
            }

            return View(technicianViewModel);
        }

        public async Task<IActionResult> DeleteTechnician(string id)
        {
            await _technicianService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> UpdateTechnician(string id)
        {
            var technician = await _technicianService.GetByIdAsync(id);
            if (technician == null)
            {
                return NotFound();
            }
            return View(technician);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTechnician(TechnicianViewModel technicianViewModel)
        {
            if (ModelState.IsValid)
            {
                await _technicianService.UpdateAsync(technicianViewModel);
                return RedirectToAction("GetAllTechnicians");
            }
            return View(technicianViewModel);
        }
    }
}

