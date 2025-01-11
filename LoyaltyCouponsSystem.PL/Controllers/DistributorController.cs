using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
using LoyaltyCouponsSystem.BLL.ViewModel.Technician;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.PL.Controllers
{
    public class DistributorController : Controller
    {
        private readonly IDistributorService _distributorService;

        public DistributorController(IDistributorService distributorService)
        {
            _distributorService = distributorService;
        }

        [HttpGet]
        public IActionResult AddDistributor()
        {
            var distributorViewModel = new DistributorViewModel
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
            return View(distributorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDistributor(DistributorViewModel distributorViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _distributorService.AddAsync(distributorViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllDistributors");
                }
                ModelState.AddModelError("", "Unable to add distributor. Please try again.");
            }
            distributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();
            return View(distributorViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDistributor(int id)
        {
            var distributorViewModel = await _distributorService.GetByIdAsync(id);
            if (distributorViewModel != null)
            {
                distributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();
                return View(distributorViewModel);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDistributor(DistributorViewModel distributorViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _distributorService.UpdateAsync(distributorViewModel);
                if (result)
                {
                    return RedirectToAction("GetAllDistributors");
                }
                ModelState.AddModelError("", "Unable to update distributor. Please try again.");
            }
            distributorViewModel.Customers = await _distributorService.GetCustomersForDropdownAsync();
            return View(distributorViewModel);
        }
    }
}
