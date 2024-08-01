
﻿using Microsoft.EntityFrameworkCore;
using MyLibrary.Data;
using MyLibrary.Models;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
namespace MyLibrary.Repositories
{
    public class Crud
    {
        private readonly MyLibraryContext _context;

        public Crud(MyLibraryContext ctx)
        {
            _context = ctx;
        }

        /// <summary>
        /// Generic method to add an entity to the database
        /// returns id of item created or -1
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public async Task<int> Add(Book book)
        {
            _context.Book.Add(book);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0 ? book.Id : -1;
        }

        // Optionally, to leverage IQueryable and defer execution with database-side filtering:
        public async Task<List<Book>> FindByAsync(Expression<Func<Book, bool>> predicate) =>
            await _context.Book.Where(predicate).ToListAsync();

        public async Task<List<Shelf>> FindByAsyncShelf(Expression<Func<Shelf, bool>> predicate) =>
    await _context.Shelf.Where(predicate).ToListAsync();

        // FindByAsync(user=>user.FirstName=="DUDU" && user.Phone=="05555")
        // FindByAsync(user=>string.IsullOrempty(user.Email))

        public async Task<int> Update(Book model)
        {
            _context.Book.Update(model);
            return await _context.SaveChangesAsync();
        }

        // Generic method to delete an entity from the database
        public async Task<int> Delete(Book model)
        {
            _context.Book.Remove(model);
            return await _context.SaveChangesAsync();
        }

        // Generic method to retrieve all entities of a type
        public async Task<List<Book>> GetAll() =>
            await _context.Book.ToListAsync() ?? new List<Book>();

        public async Task<List<Book>> GetPaginated(int offset, int limit) =>
            await _context.Book
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

/*        public static bool TooLow(int bookHight, int shelfId)
        {
           
            Crud crud = new Crud(MyLibraryContext);
            List<Shelf> shelfHeight = FindByAsyncShelf(shelf => shelf.Id == shelfId);
            return shelfHeight[0].Height - 10 > bookHight;

        }*/

        // FindByAsync(user=>user.FirstName=="DUDU" && user.Phone=="05555")
        // FindByAsync(user=>string.IsullOrempty(user.Email))


    }
}