namespace TourBookingAdvanced.ViewModels;

public class BookingViewModel
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = "";

    public DateTime? BookingDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int TotalPassengers { get; set; }
}