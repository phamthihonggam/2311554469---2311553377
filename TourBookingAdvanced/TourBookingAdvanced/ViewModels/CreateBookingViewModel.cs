using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TourBookingAdvanced.ViewModels;

public class CreateBookingViewModel
{
    [Required]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = "";

    [Required]
    [Display(Name = "Tour")]
    public int TourId { get; set; }

    [Required]
    [Range(1, 100)]
    [Display(Name = "Passengers")]
    public int PassengersCount { get; set; }

    public List<SelectListItem> Tours { get; set; } = new();
}