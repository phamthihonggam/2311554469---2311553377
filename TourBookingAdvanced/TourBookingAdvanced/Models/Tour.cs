using System;
using System.Collections.Generic;

namespace TourBookingAdvanced.Models;

public partial class Tour
{
    public int Id { get; set; }

    public string TourName { get; set; } = null!;

    public decimal Price { get; set; }

    public int AvailableSlots { get; set; }

    public string? ImagePaths { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual TourCategory Category { get; set; } = null!;
}
