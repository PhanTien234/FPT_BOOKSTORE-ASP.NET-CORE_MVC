using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPT_BOOKSTORE.Models;

public class Book
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [Required(ErrorMessage = "Price is required")]
    public double Price { get; set; }
    [Required]
    public string Author { get; set; }
    
    // [Required]
    public string ImgUrl { get; set; }
    [Required]
    public int NuPages { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity { get; set; }
    [Required]
    public int CategoryId { get; set; }

    // link to category
    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
    public string CreateBy { get; set; }
}
    
