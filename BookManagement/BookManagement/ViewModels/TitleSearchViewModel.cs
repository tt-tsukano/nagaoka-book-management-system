using BookManagement.Models;
namespace BookManagement.ViewModels
{
    public class BookSearchResultViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishedYear { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool BorrowedStatus { get; set; }
        public int? UserId { get; set; }
        public string BorrowerEmail { get; set; }  // 追加
    }

    public class TitleSearchViewModel
    {
        //タイトル検索用パラメータ
        public string Title { get; set; }

        //検索結果記事の一覧を格納するList
        public List<BookSearchResultViewModel> Results { get; set; }

        // ユーザーが借りている書籍の一覧を格納するList
        public List<Book> UserBooks { get; set; }
    }
}
