using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookManagement.Models;

namespace BookManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Modelをプロパティとして設定
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        // Fluent APIを使ったモデル定義の追加
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            // 論理削除のためのフィルター
            modelbuilder.Entity<Book>()
                .HasQueryFilter(b => !b.IsDeleted);
        }
    }
}
