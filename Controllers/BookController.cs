using FPT_BOOKSTORE.Data;
using FPT_BOOKSTORE.Models;
using FPT_BOOKSTORE.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.Controllers;

public class BookController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public BookController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
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
        var upsertVM = new BookCreateRequest()
        {
            Book = new Book(),
            CategoryList = _context.Categories.ToList()
                .ToList()
                .Select(_ => new SelectListItem()
                {
                    Value = _.Id.ToString(),
                    Text = _.Name

                }).ToList()
        };
        return View(upsertVM);
    }

// POST: Books/Create
// To protect from overposting attacks, enable the specific properties you want to bind to, for
// more details, see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateRequest input)
    {
       
        if (!ModelState.IsValid)
        {
            var webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                var fileName = Guid.NewGuid();
                var uploads = Path.Combine(webRootPath, @"img/books");
                var extension = Path.GetExtension(files[0].FileName);
                if (input.Book.Id != 0)
                {
                    var productDb = _context.Books.AsNoTracking()
                        .Where(b => b.Id == input.Book.Id).First();
                    if (productDb.ImgUrl != null && input.Book.Id != 0)
                    {
                        var imagePath = Path.Combine(webRootPath, productDb.ImgUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                    }
                }

                using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStreams);
                }

                input.Book.ImgUrl = @"/img/books/" + fileName + extension;
            }

            else
            {
                input.CategoryList = CategorySelectListItems();
                return View(input);
            }

            _context.Books.Add(input.Book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            
        }
        
        Console.WriteLine("================> Some thing went wrong!");
        input.CategoryList = CategorySelectListItems();
        return View(input);

    }
    
    
    // method for category select list VM
    public List<SelectListItem> CategorySelectListItems()
    {
        var categories = _context.Categories.ToList();
        var selectListItems = new List<SelectListItem>();
        foreach (var category in categories)
        {
            selectListItems.Add(new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.Name
            });
        }
        return selectListItems;
    }


    // // GET: Books/Edit/5
    // public async Task<IActionResult> Edit(int? id)
    // {
    //     if (id == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var book = await _context.Books.FindAsync(id);
    //     if (book == null)
    //     {
    //         return NotFound();
    //     }
    //     return View(book);
    // }
    //
    // // POST: Books/Edit/5
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Author,ImgUrl,NumPages,Quantity")] Book book, IFormFile file)
    // {
    //     if (id != book.Id)
    //     {
    //         return NotFound();
    //     }
    //
    //     if (ModelState.IsValid)
    //     {
    //         try
    //         {
    //             if (file != null && file.Length > 0)
    //             {
    //                 var filePath = Path.Combine("wwwroot/images", Path.GetFileName(file.FileName));
    //                 using (var stream = new FileStream(filePath, FileMode.Create))
    //                 {
    //                     await file.CopyToAsync(stream);
    //                 }
    //                 book.ImgUrl = "/images/" + file.FileName;
    //             }
    //             _context.Update(book);
    //             await _context.SaveChangesAsync();
    //         }
    //         catch (DbUpdateConcurrencyException)
    //         {
    //             if (!BookExists(book.Id))
    //             {
    //                 return NotFound();
    //             }
    //             else
    //             {
    //                 throw;
    //             }
    //         }
    //         var category = await _context.Categories.FindAsync(book.CategoryId);
    //         return RedirectToAction(nameof(Index), new { categoryId = category.Id });
    //     }
    //     return View(book);
    // }
    //
    // // GET: Books/Delete/5
    // public async Task<IActionResult> Delete(int? id)
    // {
    //     if (id == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var book = await _context.Books
    //         .Include(b => b.Category)
    //         .FirstOrDefaultAsync(m => m.Id == id);
    //     if (book == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return View(book);
    // }
    //
    // // POST: Books/Delete/5
    // [HttpPost, ActionName("Delete")]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> DeleteConfirmed(int id)
    // {
    //     var book = await _context.Books.FindAsync(id);
    //     _context.Books.Remove(book);
    //     await _context.SaveChangesAsync();
    //     var category = await _context.Categories.FindAsync(book.CategoryId);
    //     return RedirectToAction(nameof(Index), new { categoryId = category.Id });
    // }
    //
    // private bool BookExists(int id)
    // {return _context.Books.Any(e => e.Id == id);
    // }
}