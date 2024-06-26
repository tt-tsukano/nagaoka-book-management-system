using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // データベースとのやりとりを担当するDbContextクラスを用意
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        // コンストラクタでDbContextクラスを受け取る
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
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

    // Confirmアクション GET時
    // 引数として該当するBookのidを受け取る
    public IActionResult Confirm(int? id)
    {
        // idがnullの場合はエラーを返す
        if (id == null)
        {
            return NotFound();
        }

        // idに該当するBookを取得
        var book = _context.Books.FirstOrDefault(x => x.Id == id);

        // ビューにBookを渡して表示
        return View(book);
    }

    // Confirmアクション POST時
    // 引数として該当するBookのidを受け取る
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirm(int id)
    {
        // idに該当するBookを取得
        var book = _context.Books.FirstOrDefault(x => x.Id == id);

        // BookのBorrowedStatusをtrueに変更
        book.BorrowedStatus = true;

        // Indexアクションにリダイレクト
        return RedirectToAction("Index");
    }
}
