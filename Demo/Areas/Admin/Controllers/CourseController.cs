using Demo.Service.Services.Interfaces;
using Demo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Demo.AdminSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICourseCategoryService _categoryService;
        private readonly IInstructorService _instructorService;
        private readonly IWebHostEnvironment _environment;

        public CourseController(ICourseService courseService, ICourseCategoryService categoryService, IInstructorService instructorService,  IWebHostEnvironment environment)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _instructorService = instructorService;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            ViewBag.Keyword = keyword;

            var courses = await _courseService.GetAllAsync(keyword);
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _courseService.GetByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CourseViewModel
            {
                CourseCategories = await _categoryService.GetAllAsync(),

                Instructors = await _instructorService.GetAllAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                model.CourseCategories = await _categoryService.GetAllAsync();

                model.Instructors = await _instructorService.GetAllAsync();

                return View(model);
            }

            try
            {
                if (imageFile != null)
                {
                    model.ImagePath =
                        await SaveImageAsync(imageFile);
                }

                await _courseService.CreateAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                model.CourseCategories =
                    await _categoryService.GetAllAsync();

                model.Instructors =
                    await _instructorService.GetAllAsync();

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model =
                await _courseService.GetByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            model.CourseCategories =
                await _categoryService.GetAllAsync();

            model.Instructors =
                await _instructorService.GetAllAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                model.CourseCategories = await _categoryService.GetAllAsync();

                model.Instructors = await _instructorService.GetAllAsync();

                return View(model);
            }

            try
            {
                if (imageFile != null)
                {
                    model.ImagePath = await SaveImageAsync(imageFile);
                }

                await _courseService.UpdateAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                model.CourseCategories =
                    await _categoryService.GetAllAsync();

                model.Instructors =
                    await _instructorService.GetAllAsync();

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseService.DeleteAsync(id);

                return Json(new
                {
                    success = true,
                    message = "Xóa khóa học thành công."
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            try
            {
                await _courseService.PublishAsync(id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                await _courseService.CloseAsync(id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public IActionResult CreateLesson(int courseId)
        {
            return View(new LessonViewModel
            {
                CourseId = courseId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(LessonViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _courseService.AddLessonAsync(model);

                return RedirectToAction(nameof(Details), new { id = model.CourseId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditLesson(int id)
        {
            var model =
                await _courseService.GetLessonByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(LessonViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _courseService.UpdateLessonAsync(model);

                return RedirectToAction(nameof(Details), new { id = model.CourseId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int id, int courseId)
        {
            try
            {
                await _courseService.DeleteLessonAsync(id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Only jpg, jpeg, png and webp files are allowed.");
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                throw new InvalidOperationException("Image size cannot exceed 2 MB.");
            }

            var folder = Path.Combine(
                _environment.WebRootPath, "uploads", "courses");

            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var fullPath = Path.Combine(folder, fileName);

            using var stream =
                new FileStream(fullPath, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/uploads/courses/{fileName}";
        }
    }
}