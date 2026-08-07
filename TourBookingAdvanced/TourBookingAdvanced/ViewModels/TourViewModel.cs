using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourBookingAdvanced.ViewModels;

public class TourViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tour Name is required")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = "";

    [Required(ErrorMessage = "Price is required")]
    [Range(1, 100000000)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Available Slots is required")]
    [Range(1, 1000)]
    [Display(Name = "Available Slots")]
    public int AvailableSlots { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    // Upload nhiều ảnh
    public List<IFormFile>? Images { get; set; }
}