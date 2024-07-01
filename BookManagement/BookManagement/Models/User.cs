using Microsoft.AspNetCore.Identity;

namespace BookManagement.Models
{
    public class User : IdentityUser<int>
    {
        // IdentityUserがもってるので省略

        // Bookテーブルとのリレーション
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
