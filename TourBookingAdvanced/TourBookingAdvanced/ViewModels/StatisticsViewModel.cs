namespace TourBookingAdvanced.ViewModels;

public class StatisticsViewModel
{
    public decimal TotalRevenue { get; set; }

    public int TotalBookings { get; set; }

    public List<TopTourViewModel> TopTours { get; set; } = new();
}

public class TopTourViewModel
{
    public string TourName { get; set; } = "";

    public int TotalPassengers { get; set; }
}