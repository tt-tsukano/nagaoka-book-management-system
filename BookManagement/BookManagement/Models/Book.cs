using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="タイトルを入力してください。")]
        public string Title { get; set; }
        //Authorを追加
        [Required(ErrorMessage ="著者名を入力してください。複数いる場合は、1名で良いです。")]
        public string Author { get; set; }
        public bool BorrowedStatus { get; set; } = false;
        public Date? BorrowedDate { get; set; }
        public Date? ReturnDate { get; set; }

        [Required(ErrorMessage ="出版年を記入してください。")]
        public int PublishedYear { get; set; }

        // 外部キー
        public int UserId { get; set; } = 0;

        // 紐づくUserテーブルのデータ
        public User User { get; set; } = null;
    }
}
