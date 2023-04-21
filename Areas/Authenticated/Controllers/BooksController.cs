using ExcelDataReader;
using FPT_BOOKSTORE.Data;
using FPT_BOOKSTORE.Models;
using FPT_BOOKSTORE.Utility.cs;
using FPT_BOOKSTORE.VM;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.Controllers;

[Area(Constraintt.AuthenticatedArea)]
[Authorize(Roles = Constraintt.StoreOwnerRole)]

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BooksController(ApplicationDbContext context,  IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    
     // GET
        // --------------------INDEX-------------------
        [HttpGet]
        public IActionResult Index()
        {
            var books = _context.Books
                .Include(x => x.Category).ToList();
            return View(books);
        }


        // -------------------DELETE--------------------
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var objBook = _context.Books.Find(id);
            _context.Books.Remove(objBook);
            _context.SaveChanges();

            TempData["DeleteBoMessage"] = "Deleted Book Successfully!";
            TempData["ShowMessage"] = true; //Set flag to show message in the view
            return RedirectToAction(nameof(Index));
        }


        // -------------------UPSERT----------------------
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var bookVm = new BookVm();

            bookVm.Categories = CategorySelectListItems();


            if (id == null)
            {
                bookVm.Book = new Book();
                return View(bookVm);
            }

            var book = _context.Books.Find(id);
            bookVm.Book = book;
            return View(bookVm);
        }


        [HttpPost]
        public IActionResult Upsert(BookVm bookVm)
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


            if (bookVm.Book.Id == 0 || bookVm.Book.Id == null)
            {
                _context.Books.Add(bookVm.Book);
            }
            else
            {
                _context.Books.Update(bookVm.Book);
            }
            _context.SaveChanges();
            TempData["CreateBoMessage"] = "Created Book Successfully!";
            TempData["ShowMessage"] = true; //Set flag to show message in the view
            return RedirectToAction(nameof(Index));

            // provide data for the categories list
            // bookVm.;Categories = CategorySelectListItems();

        }

        // method for category select list VM
        private IEnumerable<SelectListItem> CategorySelectListItems()
        {
            var categories = _context.Categories
                .Where(c => c.Status == Category.StatusCategory.Approve)
                .ToList();

            // for each book
            var result = categories
                .Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            return result;
        }

        // // Upload Book
        // public IActionResult UploadExcel(IFormFile file)
        // {
        //     if (file == null)
        //     {
        //         return RedirectToAction(nameof(Index));
        //     }
        //
        //     var path = Path.Combine(_environment.WebRootPath, "uploads");
        //     if (!Directory.Exists(path))
        //     {
        //         Directory.CreateDirectory(path);
        //     }
        //
        //     string fileName = Path.GetFileName(file.FileName);
        //     string filePath = Path.Combine(path, fileName);
        //
        //     using (FileStream stream = new FileStream(filePath, FileMode.Create))
        //     {
        //         file.CopyTo(stream);
        //     }
        //
        //     using var streamFile = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
        //     using var reader = ExcelReaderFactory.CreateReader(streamFile);
        //     
        //     
        //     while (reader.Read())
        //     {
        //         var category = _context.Categories.FirstOrDefault(c => c.Name == reader.GetValue(3).ToString());
        //         if (category == null)
        //         { 
        //             continue;
        //         }
        //         
        //         var book = new Book()
        //         {
        //             Title = reader.GetValue(0).ToString(),
        //             Description = reader.GetValue(1).ToString(),
        //             Price = Convert.ToDouble(reader.GetValue(2).ToString()),
        //             CategoryId = category.Id,
        //             Author = reader.GetValue(4).ToString(),
        //             NuPages = Convert.ToInt32(reader.GetValue(5).ToString()),
        //             ImgUrl = reader.GetValue(6).ToString(),
        //         };
        //
        //         _context.Books.Add(book);
        //     }
        //
        //     _context.SaveChanges();
        //     return RedirectToAction(nameof(Index));
        //
        // }
    
    
    
        
}