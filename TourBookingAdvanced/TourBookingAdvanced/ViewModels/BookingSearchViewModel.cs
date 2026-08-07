using Microsoft.AspNetCore.Mvc.Rendering;

namespace TourBookingAdvanced.ViewModels;

public class BookingSearchViewModel
{
    public string? Keyword { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public List<BookingViewModel> Bookings { get; set; } = new();

    public int TotalBookings { get; set; }

    public decimal TotalRevenue { get; set; }
}