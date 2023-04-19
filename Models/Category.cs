using System.ComponentModel.DataAnnotations;

namespace FPT_BOOKSTORE.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string  Name { get; set; }
    [Required]
    public string Description { get; set; }

    public enum StatusCategory
    {
        Approve,
        Reject,
        Pending
    }
    
    [Required]
    public StatusCategory Status { get; set; }

}