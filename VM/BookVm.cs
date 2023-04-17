using FPT_BOOKSTORE.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPT_BOOKSTORE.VM;

public class BookVm
{
    public Book Book { get; set; }
    public IEnumerable<SelectListItem> Categories { get; set; }
}