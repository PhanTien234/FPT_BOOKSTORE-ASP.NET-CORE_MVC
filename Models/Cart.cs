using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FPT_BOOKSTORE.Models;

public class Cart
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public int BookId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Count must be greater than 0")]
    [Required(ErrorMessage = "Count is required")]
    public int Count { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }
    [ForeignKey("BookId")]
    public Book Book { get; set; }
    [NotMapped] public double Price { get; set; }

    public Cart()
    {
        Count = 1;
    }
}