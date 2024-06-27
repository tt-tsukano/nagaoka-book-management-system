using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using BookManagement.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BookManagement.ViewModels;
using System.Text;
using System.Security.Cryptography;
using System.Text;

namespace BookManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    // パスワードをハッシュ化するメソッド
    public string HashPassword(string password)
    {
        // SHA256のハッシュ値を計算するクラスを作成
        // SHA256は入力されたデータからハッシュ値を生成するアルゴリズムの一つ
        // usingブロックを使うことで、処理が終了したら自動的にリソースを解放する
        using (SHA256 sha256 = SHA256.Create())
        {
            // パスワードをバイト配列に変換
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // ハッシュ計算
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            // ハッシュ値を文字列に変換して返す
            return Convert.ToBase64String(hashBytes);
        }
    }


public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookslist = await _context.Books.ToListAsync();

        //TitleSearchViewModelのResultsに記事一覧を格納
        var viewModel = new TitleSearchViewModel
        {
            Results = bookslist
        };
        
        //bookslistではなく、viewModelをviewに返す
        return View(viewModel);
    }


    //検索ボタンを押した後の操作
    [HttpPost]
    public async Task<IActionResult> Index(TitleSearchViewModel titleSearchViewModel)
    {
        IQueryable<Book> books = _context.Books;

        //タイトルの全文一致
        if (!string.IsNullOrEmpty(titleSearchViewModel.Title))
        {
            books = books.Where(b => b.Title.Contains(titleSearchViewModel.Title));
        }

        titleSearchViewModel.Results = await books.ToListAsync();

        return View(titleSearchViewModel);
    }

    // GET: Home/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Home/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    //Loginメソッドの引数にEmailとPasswordを追加
    public async Task<IActionResult> Login(string Email, string Password)
    {
        //ユーザーが入力したパスワードをハッシュ化
         Password = HashPassword(Password);

        //EmailとPasswordが一致するUserを取得
        var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == Email);// && m.PasswordHash == Password);

        //Userがnullの場合はNotFoundを返す
        if (user == null)
        {
            return NotFound();
        }

        //UserをIndexのViewに渡して表示
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
