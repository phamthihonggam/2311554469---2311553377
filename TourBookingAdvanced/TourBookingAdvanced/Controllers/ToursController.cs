using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourBookingAdvanced.Models;
using TourBookingAdvanced.ViewModels;

namespace TourBookingAdvanced.Controllers;

public class ToursController : Controller
{
    private readonly TourBookingAdvancedDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ToursController(
        TourBookingAdvancedDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    //==================== INDEX ====================

    public async Task<IActionResult> Index()
    {
        var tours = await _context.Tours
            .Include(x => x.Category)
            .ToListAsync();

        return View(tours);
    }

    //==================== DETAILS ====================

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var tour = await _context.Tours
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (tour == null)
            return NotFound();

        return View(tour);
    }

    //==================== CREATE (GET) ====================

    public IActionResult Create()
    {
        ViewBag.CategoryId = new SelectList(
            _context.TourCategories,
            "Id",
            "Name");

        return View(new TourViewModel());
    }

    //==================== CREATE (POST) ====================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TourViewModel vm)
    {
        ViewBag.CategoryId = new SelectList(
            _context.TourCategories,
            "Id",
            "Name",
            vm.CategoryId);

        if (!ModelState.IsValid)
            return View(vm);

        List<string> images = new();

        if (vm.Images != null)
        {
            string folder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "tours");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var file in vm.Images)
            {
                if (file.Length <= 0)
                    continue;

                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(file.FileName);

                string path = Path.Combine(folder, fileName);

                using FileStream stream =
                    new(path, FileMode.Create);

                await file.CopyToAsync(stream);

                images.Add(fileName);
            }
        }

        Tour tour = new()
        {
            TourName = vm.TourName,
            Price = vm.Price,
            AvailableSlots = vm.AvailableSlots,
            CategoryId = vm.CategoryId,
            ImagePaths = string.Join(";", images)
        };

        _context.Tours.Add(tour);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Tour created successfully.";

        return RedirectToAction(nameof(Index));
    }
    //==================== EDIT (GET) ====================

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var tour = await _context.Tours.FindAsync(id);

        if (tour == null)
            return NotFound();

        ViewBag.CategoryId = new SelectList(
            _context.TourCategories,
            "Id",
            "Name",
            tour.CategoryId);

        var vm = new TourViewModel
        {
            Id = tour.Id,
            TourName = tour.TourName,
            Price = tour.Price,
            AvailableSlots = tour.AvailableSlots,
            CategoryId = tour.CategoryId
        };

        return View(vm);
    }

    //==================== EDIT (POST) ====================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TourViewModel vm)
    {
        if (id != vm.Id)
            return NotFound();

        ViewBag.CategoryId = new SelectList(
            _context.TourCategories,
            "Id",
            "Name",
            vm.CategoryId);

        if (!ModelState.IsValid)
            return View(vm);

        var tour = await _context.Tours.FindAsync(id);

        if (tour == null)
            return NotFound();

        List<string> images = new();

        if (!string.IsNullOrWhiteSpace(tour.ImagePaths))
        {
            images = tour.ImagePaths
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        if (vm.Images != null && vm.Images.Any())
        {
            string folder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "tours");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var file in vm.Images)
            {
                if (file.Length <= 0)
                    continue;

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(file.FileName);

                string path = Path.Combine(folder, fileName);

                using FileStream stream =
                    new(path, FileMode.Create);

                await file.CopyToAsync(stream);

                images.Add(fileName);
            }
        }

        tour.TourName = vm.TourName;
        tour.Price = vm.Price;
        tour.AvailableSlots = vm.AvailableSlots;
        tour.CategoryId = vm.CategoryId;
        tour.ImagePaths = string.Join(";", images);

        _context.Update(tour);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Tour updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    //==================== DELETE (GET) ====================

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var tour = await _context.Tours
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (tour == null)
            return NotFound();

        return View(tour);
    }

    //==================== DELETE (POST) ====================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tour = await _context.Tours.FindAsync(id);

        if (tour != null)
        {
            _context.Tours.Remove(tour);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Tour deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    //==================== EXISTS ====================

    private bool TourExists(int id)
    {
        return _context.Tours.Any(x => x.Id == id);
    }
}
