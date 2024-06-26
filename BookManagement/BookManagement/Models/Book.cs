namespace BookManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool BorrowedStatus { get; set; } = false;
        public DateTime BorrowedDate { get; set; } = DateTime.Now;
        public DateTime ReturnDate { get; set; }
        public int PublishedYear { get; set; }
        public int UserId { get; set; }
    }
}
