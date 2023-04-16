using FPT_BOOKSTORE.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace FPT_BOOKSTORE.VM;

public class BookCreateRequest
{
    [Required] public Book Book { get; set; }
    
    [Required] public List<SelectListItem> CategoryList { get; set; }
    
    [Required] public IFormFile File { get; set; }

}