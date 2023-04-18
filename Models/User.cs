using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace FPT_BOOKSTORE.Models;

public class User : IdentityUser
{
    public User()
    {
        CreatedAt = DateTime.UtcNow;
    }
    
    [Required]
    public string FullName { get; set; }
    [Required]
    public string HomeAddress { get; set; }
    
    [NotMapped] public string Role { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdateAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDelete { get; set; }
}