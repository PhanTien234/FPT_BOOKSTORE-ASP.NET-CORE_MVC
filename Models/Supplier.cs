using System.ComponentModel.DataAnnotations;

namespace FPT_BOOKSTORE.Models;

public class Supplier
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
}