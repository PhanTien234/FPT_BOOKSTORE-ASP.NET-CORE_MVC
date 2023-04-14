using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPT_BOOKSTORE.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public DateTime Order_Date { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public double Total { get; set; }
   
    //link to User
    [ForeignKey("UserId")]
    public User User { get; set; }
}