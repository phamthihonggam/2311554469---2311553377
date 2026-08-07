using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourBookingAdvanced.Models;
using TourBookingAdvanced.ViewModels;

namespace TourBookingAdvanced.Controllers;

public class StatisticsController : Controller
{
    private readonly TourBookingAdvancedDbContext _context;

    public StatisticsController(TourBookingAdvancedDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new StatisticsViewModel();

        vm.TotalRevenue = await _context.Bookings
            .SumAsync(x => x.TotalAmount ?? 0);

        vm.TotalBookings = await _context.Bookings
            .CountAsync();

        vm.TopTours = await _context.BookingDetails
            .GroupBy(x => x.Tour.TourName)
            .Select(g => new TopTourViewModel
            {
                TourName = g.Key,
                TotalPassengers = g.Sum(x => x.PassengersCount)
            })
            .OrderByDescending(x => x.TotalPassengers)
            .Take(3)
            .ToListAsync();

        return View(vm);
    }
}