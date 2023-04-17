using FPT_BOOKSTORE.Data;
using FPT_BOOKSTORE.Models;
using FPT_BOOKSTORE.Utility.cs;
using FPT_BOOKSTORE.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.Controllers;

[Area(Constraintt.AuthenticatedArea)]
public class BookController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BookController(ApplicationDbContext context,  IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    
    // method for category select list VM
            private IEnumerable<SelectListItem> CategorySelectListItems()
            {
                var categories = _context.Categories.ToList();
    
                // for each book
                var result = categories
                    .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
    
                return result;
            }
    
    // GET: Books
    public IActionResult Index()
    {
        var books = _context.Books.Include(b => b.Category).ToList();
        return View(books);
    }
    //
    // // GET: Books/Details/5
    // public async Task<IActionResult> Details(int? id)
    // {
    //     if (id == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var book = await _context.Books.Include(b => b.Category)
    //         .FirstOrDefaultAsync(m => m.Id == id);
    //
    //     if (book == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return View(book);
    // }

    // GET: Books/Create
    public IActionResult Create()
    {
        var bookVm = new BookVm();

        bookVm.Categories = CategorySelectListItems();
        
        bookVm.Book = new Book();
        return View(bookVm);
                      
    }

// POST: Books/Create
// To protect from overposting attacks, enable the specific properties you want to bind to, for
// more details, see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookVm bookVm)
    {
        if (ModelState.IsValid)
        {
            bookVm.Categories = CategorySelectListItems();
            return View(bookVm);
        }

        var webRootPath = _environment.WebRootPath;
        var files = HttpContext.Request.Form.Files;
        if (files.Count > 0)
        {
            var fileName = Guid.NewGuid();
            var uploads = Path.Combine(webRootPath, @"img/books");
            var extension = Path.GetExtension(files[0].FileName);
            if (bookVm.Book.Id != 0)
            {
                var productDb = _context.Books.AsNoTracking()
                    .Where(b => b.Id == bookVm.Book.Id).First();
                if (productDb.ImgUrl != null && bookVm.Book.Id != 0)
                {
                    var imagePath = Path.Combine(webRootPath, productDb.ImgUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }
            }

            using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                files[0].CopyTo(filesStreams);
            }

            bookVm.Book.ImgUrl = @"/img/books/" + fileName + extension;
        }

        else
        {
            bookVm.Categories = CategorySelectListItems();
            return View(bookVm);
        }

        _context.Books.Add(bookVm.Book);

        _context.SaveChanges();

        // provide data for the categories list
        // bookVm.;Categories = CategorySelectListItems();

        return RedirectToAction(nameof(Index));
    }
}