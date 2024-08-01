using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Data;
using MyLibrary.Models;
using MyLibrary.ViewModel;

namespace MyLibrary.Controllers
{
    public class BookSetsController : Controller
    {
        private readonly MyLibraryContext _context;

        public BookSetsController(MyLibraryContext context)
        {
            _context = context;
        }

        // GET: BookSets
        public async Task<IActionResult> Index()
        {
            var myLibraryContext = _context.BookSet.Include(b => b.Shelf);
            return View(await myLibraryContext.ToListAsync());
        }

        // GET: BookSets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookSet = await _context.BookSet
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookSet == null)
            {
                return NotFound();
            }

            return View(bookSet);
        }

        // GET: BookSets/Create
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

        // POST: BookSets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddBookSet addBookSet)
        {
            bool reimainSpace = NoSpaceOnShelf(addBookSet.bookSet.Width, addBookSet.bookSet.ShelfId);
            bool goodHeight = TooHighShelf(addBookSet.bookSet.Height, addBookSet.bookSet.ShelfId);
            bool low = TooLowShelf(addBookSet.bookSet.Height, addBookSet.bookSet.ShelfId);
            bool correctShelf = MatchGenreShelf(addBookSet.bookSet.ShelfId, Convert.ToInt32(addBookSet.genre.Name));
            List<Genre> genres = _context.Genre.ToList();
            var shelves = _context.Shelf.Include(x => x.Library).ThenInclude(y => y.Genre).ToList();
            ViewBag.Genres = genres;
            ViewBag.Shelves = shelves;
            ModelState.Remove("bookSet.Shelf");
            ModelState.Remove("genre.Name");
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id");
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            if (ModelState.IsValid)
            {
                if (!reimainSpace)
                {
                    ModelState.AddModelError(nameof(addBookSet.bookSet.Width), "There is no place on this shelf. Try other shelf or add another");
                    return View(addBookSet);
                }
                if (low)
                {
                    TempData["ErrorMessage"] = "This book is too high. Try other shelf";
                    return View(addBookSet);
                }
                if (!correctShelf)
                {
                    TempData["ErrorMessage"] = "this shelf is not in this genre. check the table on the right";
         

                    return View(addBookSet);
                }
                if (goodHeight)
                {
                    TempData["ErrorMessage"] = "Your book is too low. but still we'll recieve it";
                }
                else
                {
                    TempData["ErrorMessage"] = "good!";
                }
                _context.Add(addBookSet.bookSet);
                await _context.SaveChangesAsync();
                return View(addBookSet);
            }
/*            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", addBookSet.bookSet.ShelfId);
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");*/
            return View(addBookSet);
        }

        // GET: BookSets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookSet = await _context.BookSet.FindAsync(id);
            if (bookSet == null)
            {
                return NotFound();
            }
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", bookSet.ShelfId);
            return View(bookSet);
        }

        // POST: BookSets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Width,Height,ShelfId")] BookSet bookSet)
        {
            if (id != bookSet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookSetExists(bookSet.Id))
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
            ViewData["ShelfId"] = new SelectList(_context.Shelf, "Id", "Id", bookSet.ShelfId);
            return View(bookSet);
        }

        // GET: BookSets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookSet = await _context.BookSet
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookSet == null)
            {
                return NotFound();
            }

            return View(bookSet);
        }

        // POST: BookSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookSet = await _context.BookSet.FindAsync(id);
            if (bookSet != null)
            {
                _context.BookSet.Remove(bookSet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookSetExists(int id)
        {
            return _context.BookSet.Any(e => e.Id == id);
        }
        private bool TooHighShelf(int bookHight, int shelfId)
        {
            Shelf shelf = _context.Shelf.Where(shelf => shelf.Id == shelfId).ToList()[0];
            return shelf.Height - 10 > bookHight;
        }
        private bool MatchGenreShelf(int shelfId, int genreId)
        {
            var genreName = _context.Genre.Where(x => x.Id == genreId).ToList();
            var shelf = _context.Shelf.Include(x => x.Library).ThenInclude(y => y.Genre)
                .FirstOrDefault(s => s.Id == shelfId);
            string f = shelf.Library.Genre.Name;

            return f == genreName[0].Name;
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
        private bool TooLowShelf(int bookHight, int shelfId)
        {
            Shelf shelf = _context.Shelf.Where(shelf => shelf.Id == shelfId).ToList()[0];
            return shelf.Height < bookHight;
        }
    }
}

