namespace rezervation.Models
{
    public class HomepageViewModel
    {
        public string? BookName { get; set; }

        public Book? SearchResult { get; set; }

        public List<Book> PopularBooks { get; set; } = new();
    }
}
