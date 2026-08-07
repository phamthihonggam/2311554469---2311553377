using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourBookingAdvanced.Models;
using TourBookingAdvanced.ViewModels;

namespace TourBookingAdvanced.Controllers;

public class BookingsController : Controller
{
    private readonly TourBookingAdvancedDbContext _context;

    public BookingsController(TourBookingAdvancedDbContext context)
    {
        _context = context;
    }

    //==================== INDEX ====================

    public async Task<IActionResult> Index(
        string? keyword,
        decimal? minAmount,
        decimal? maxAmount)
    {
        var query = _context.Bookings.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.CustomerName.Contains(keyword));
        }

        if (minAmount.HasValue)
        {
            query = query.Where(x => x.TotalAmount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(x => x.TotalAmount <= maxAmount.Value);
        }

        var bookings = await query
            .Select(x => new BookingViewModel
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                BookingDate = x.BookingDate,
                TotalAmount = x.TotalAmount,
                TotalPassengers = x.BookingDetails.Sum(d => d.PassengersCount)
            })
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        var vm = new BookingSearchViewModel
        {
            Keyword = keyword,
            MinAmount = minAmount,
            MaxAmount = maxAmount,

            Bookings = bookings,

            TotalBookings = bookings.Count,

            TotalRevenue = bookings.Sum(x => x.TotalAmount ?? 0)
        };

        return View(vm);
    }

    //==================== DETAILS ====================

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings
            .Include(x => x.BookingDetails)
            .ThenInclude(x => x.Tour)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }

    //==================== CREATE (GET) ====================

    public IActionResult Create()
    {
        var vm = new CreateBookingViewModel();

        vm.Tours = _context.Tours
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.TourName} - {x.Price:N0} VNĐ (Slots: {x.AvailableSlots})"
            })
            .ToList();

        return View(vm);
    }

    //==================== CREATE (POST) ====================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel vm)
    {
        vm.Tours = _context.Tours
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.TourName} - {x.Price:N0} VNĐ (Slots: {x.AvailableSlots})"
            })
            .ToList();

        if (!ModelState.IsValid)
            return View(vm);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var tour = await _context.Tours.FindAsync(vm.TourId);

            if (tour == null)
            {
                ModelState.AddModelError("", "Tour not found.");
                return View(vm);
            }

            if (tour.AvailableSlots < vm.PassengersCount)
            {
                ModelState.AddModelError("", "Not enough available slots.");
                return View(vm);
            }

            var booking = new Booking
            {
                CustomerName = vm.CustomerName,
                BookingDate = DateTime.Now,
                TotalAmount = tour.Price * vm.PassengersCount
            };

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            var detail = new BookingDetail
            {
                BookingId = booking.Id,
                TourId = tour.Id,
                PassengersCount = vm.PassengersCount,
                UnitPrice = tour.Price
            };

            _context.BookingDetails.Add(detail);

            tour.AvailableSlots -= vm.PassengersCount;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Success"] = "Booking created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError("", "Create booking failed.");

            return View(vm);
        }
    }
    //==================== EDIT (GET) ====================

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }

    //==================== EDIT (POST) ====================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Booking booking)
    {
        if (id != booking.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(booking);

        try
        {
            _context.Update(booking);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookingExists(booking.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    //==================== DELETE (GET) ====================

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }

    //==================== DELETE (POST) ====================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking != null)
        {
            _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    //==================== EXISTS ====================

    private bool BookingExists(int id)
    {
        return _context.Bookings.Any(x => x.Id == id);
    }
}
