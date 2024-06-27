using BookManagement.Models;
namespace BookManagement.ViewModels
{
    public class TitleSearchViewModel
    {
        //タイトル検索用パラメータ
        public string Title { get; set; }

        //検索結果記事の一覧を格納するList
        public List<Book> Results { get; set; }
    }
}
