using LoyaltyCouponsSystem.BLL.Service.Abstraction;
using LoyaltyCouponsSystem.BLL.ViewModel.Distributor;
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
        public async Task<IActionResult> AddDistributor()
        {

            var distributorViewModel = new DistributorViewModel
            {
                Customers = await _distributorService.GetCustomersForDropdownAsync()
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
