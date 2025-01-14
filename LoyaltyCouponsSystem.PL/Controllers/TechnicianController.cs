using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using Microsoft.AspNetCore.Identity;
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
                new SelectListItem { Text = "Giza", Value = "Giza" },
                new SelectListItem { Text = "Alexandria", Value = "Alexandria" },
                new SelectListItem { Text = "Dakahlia", Value = "Dakahlia" },
                new SelectListItem { Text = "Red Sea", Value = "Red Sea" },
                new SelectListItem { Text = "Beheira", Value = "Beheira" },
                new SelectListItem { Text = "Fayoum", Value = "Fayoum" },
                new SelectListItem { Text = "Sharqia", Value = "Sharqia" },
                new SelectListItem { Text = "Aswan", Value = "Aswan" },
                new SelectListItem { Text = "Assiut", Value = "Assiut" },
                new SelectListItem { Text = "Beni Suef", Value = "Beni Suef" },
                new SelectListItem { Text = "Port Said", Value = "Port Said" },
                new SelectListItem { Text = "Damietta", Value = "Damietta" },
                new SelectListItem { Text = "Ismailia", Value = "Ismailia" },
                new SelectListItem { Text = "Kafr El Sheikh", Value = "Kafr El Sheikh" },
                new SelectListItem { Text = "Matruh", Value = "Matruh" },
                new SelectListItem { Text = "Luxor", Value = "Luxor" },
                new SelectListItem { Text = "Qalyubia", Value = "Qalyubia" },
                new SelectListItem { Text = "Qena", Value = "Qena" },
                new SelectListItem { Text = "Monufia", Value = "Monufia" },
                new SelectListItem { Text = "North Sinai", Value = "North Sinai" },
                new SelectListItem { Text = "Sohag", Value = "Sohag" },
                new SelectListItem { Text = "South Sinai", Value = "South Sinai" },
                new SelectListItem { Text = "New Valley", Value = "New Valley" },
                new SelectListItem { Text = "Gharbia", Value = "Gharbia" },
                new SelectListItem { Text = "Suez", Value = "Suez" }
        }
            };
            return View(technicianViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTechnician(TechnicianViewModel technicianViewModel)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
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

