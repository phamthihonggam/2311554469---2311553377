using System;
using System.Collections.Generic;

namespace TourBookingAdvanced.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public DateTime? BookingDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
}
