using System;
using System.Collections.Generic;

namespace TourBookingAdvanced.Models;

public partial class TourCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
