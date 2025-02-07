namespace LibraryManagerWeb.DataAccess
{
    public class Magazine
    {
        public int MagazineId { get; set; }
        public required string Title { get; set; }
        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public DateTime LoadedDate { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public required Category Category { get; set; }
    }
}
