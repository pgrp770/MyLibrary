using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Data;
using MyLibrary.Models;
using MyLibrary.Repositories;
using MyLibrary.ViewModel;

namespace MyLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly MyLibraryContext _context;
        private readonly Crud crud;

        public BooksController(MyLibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var myLibraryContext = _context.Book.Include(b => b.Shelf);
            return View(await myLibraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id");
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            List<Genre> genres = _context.Genre.ToList();
            var shelves = _context.Shelf.Include(x => x.Library).ThenInclude(y => y.Genre).ToList();
            ViewBag.Genres = genres;
            ViewBag.Shelves = shelves;
            
            
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddBook addBook)
        {
            bool reimainSpace = NoSpaceOnShelf(addBook.book.Width, addBook.book.ShelfId);
            bool goodHeight = TooHighShelf(addBook.book.Height, addBook.book.ShelfId);
            bool low = TooLowShelf(addBook.book.Height, addBook.book.ShelfId);
            bool correctShelf = MatchGenreShelf(addBook.book.ShelfId, Convert.ToInt32(addBook.genre.Name));
            ModelState.Remove("book.Shelf");
            ModelState.Remove("genre.Name");

            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id");
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            List<Genre> genres = _context.Genre.ToList();
            var shelves = _context.Shelf.Include(x => x.Library).ThenInclude(y => y.Genre).ToList();
            ViewBag.Genres = genres;
            ViewBag.Shelves = shelves;
            if (ModelState.IsValid)
            {

                if (!reimainSpace)
                {
                    TempData["ErrorMessage"] = "There is no place on this shelf. Try other shelf or add another";
                    ModelState.AddModelError("book.Width", "There is no place on this shelf. Try other shelf or add another");
                    
                    return View(addBook);
                }                
                if (low)
                {
                    TempData["ErrorMessage"] = "This book is too high. Try other shelf";
                   
                    return View(addBook);
                }
                if (!correctShelf)
                {
                    TempData["ErrorMessage"] = "this shelf is not in this genre. check the table on the right";
                   
                    return View(addBook);
                }
                if (goodHeight)
                {
                    TempData["ErrorMessage"] = "Your book is too low. but still we'll recieve it";
                }
                else
                {
                    TempData["ErrorMessage"] = "good!";
                }
                _context.Add(addBook.book);
                await _context.SaveChangesAsync();
                
                return View(addBook);
            }
            
            return View(addBook);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", book.ShelfId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Width,Height,ShelfId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", book.ShelfId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }

        private bool MatchGenreShelf(int shelfId, int genreId)
        {
            var genreName = _context.Genre.Where(x => x.Id == genreId).ToList();
            var shelf = _context.Shelf.Include(x => x.Library).ThenInclude(y =>y.Genre)
                .FirstOrDefault(s=> s.Id == shelfId);
            string f = shelf.Library.Genre.Name;
            
            return f == genreName[0].Name;        
        }
        
        private  bool TooHighShelf(int bookHight, int shelfId)
        {
            Shelf shelf = _context.Shelf.Where(shelf => shelf.Id == shelfId).ToList()[0];
            return shelf.Height - 10 > bookHight;
        }

        private bool TooLowShelf(int bookHight, int shelfId)
        {
            Shelf shelf = _context.Shelf.Where(shelf => shelf.Id == shelfId).ToList()[0];
            return shelf.Height  < bookHight;
        }

        private bool NoSpaceOnShelf(int bookWithhh, int shelfId)
        {
            Shelf shelf = _context.Shelf.Where(shelf => shelf.Id == shelfId).ToList()[0];
            List<Book> books = _context.Book.Where(book => book.ShelfId == shelfId).ToList();
            List<BookSet> booksets = _context.BookSet.Where(book => book.ShelfId == shelfId).ToList();

            int booksWidth = 0;
            int bookSetsWidth = 0;

            foreach (Book book in books)
            {
                booksWidth += book.Width;
            }
            foreach (BookSet bookSet in booksets)
            {
                bookSetsWidth += bookSet.Width;
            }
            return (shelf.Width - (booksWidth + bookSetsWidth + bookWithhh)) > 0;
        }
    }
}
