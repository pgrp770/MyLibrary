using Microsoft.Extensions.Hosting;

namespace MyLibrary.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Library> Libraries { get; } = new List<Library>();
    }
}
