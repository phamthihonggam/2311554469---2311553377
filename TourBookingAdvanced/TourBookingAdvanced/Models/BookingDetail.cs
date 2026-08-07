using System;
using System.Collections.Generic;

namespace TourBookingAdvanced.Models;

public partial class BookingDetail
{
    public int BookingId { get; set; }

    public int TourId { get; set; }

    public int PassengersCount { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
