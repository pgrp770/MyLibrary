using System;

namespace MyLibrary.Models
{
    public class Library
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public List<Shelf>? Shelfes { get; set; }
    }
}
