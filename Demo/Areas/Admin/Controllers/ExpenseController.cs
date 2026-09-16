using Demo.Service.Services.Interfaces;
using Demo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Demo.AdminSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;

        public ExpenseController(IExpenseService expenseService, ICategoryService categoryService)
        {
            _expenseService = expenseService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _expenseService.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new ExpenseViewModel
            {
                Categories =
                    await _categoryService.GetAllAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllAsync();

                return View(model);
            }

            try
            {
                await _expenseService.CreateAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model.Categories = await _categoryService.GetAllAsync();

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _expenseService.GetByIdAsync(id);

            if (model == null)
                return NotFound();

            model.Categories = await _categoryService.GetAllAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            ExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllAsync();

                return View(model);
            }

            try
            {
                await _expenseService.UpdateAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _expenseService.DeleteAsync(id);

                return Json(new
                {
                    success = true,
                    message = "Expense deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}