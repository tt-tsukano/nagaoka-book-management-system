using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using BookManagement.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BookManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using BookManagement.Services;
using System.Text;

namespace BookManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    // コンストラクタでの依存性注入
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager, IEmailService emailService)
    {
        // ログ出力機能
        _logger = logger;
        // ユーザー管理機能
        _userManager = userManager;
        // メール送信機能
        _emailService = emailService;
        // データベースコンテキスト
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        try
        {
            // トランザクションスコープを使用
            using var transaction = await _context.Database.BeginTransactionAsync();

            var userBooks = await GetUserBooksAsync();

            // ToListAsyncを使用して明示的にクエリを実行
            var allBooks = await _context.Books
                .AsNoTracking() // トラッキングしない
                .Select(b => new BookSearchResultViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    PublishedYear = b.PublishedYear,
                    ReturnDate = b.ReturnDate,
                    BorrowedStatus = b.BorrowedStatus,
                    UserId = b.UserId,
                    BorrowerEmail = b.BorrowedStatus ? 
                    _userManager.FindByIdAsync(b.UserId.ToString()).Result.Email : null
                })
                .ToListAsync();

            // ユーザーが借りている書籍を除外
            var availableBooks = allBooks
                .Where(b => !userBooks.Any(ub => ub.Id == b.Id))
                .ToList();

            var viewModel = new TitleSearchViewModel
            {
                Results = availableBooks,
                UserBooks = userBooks
            };

            await transaction.CommitAsync();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "エラーが発生しました");
            return View(new TitleSearchViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Index(TitleSearchViewModel titleSearchViewModel)
    {
        var userBooks = await GetUserBooksAsync();
        IQueryable<Book> books = _context.Books;

        if (!string.IsNullOrEmpty(titleSearchViewModel.Title))
        {
            books = books.Where(b => b.Title.Contains(titleSearchViewModel.Title));
        }

        var searchResults = await books
            .Select(b => new BookSearchResultViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                PublishedYear = b.PublishedYear,
                ReturnDate = b.ReturnDate,
                BorrowedStatus = b.BorrowedStatus,
                UserId = b.UserId,
                BorrowerEmail = b.BorrowedStatus ? _userManager.FindByIdAsync(b.UserId.ToString()).Result.Email : null
            })
            .ToListAsync();

        // ユーザーが借りている書籍を除外
        var availableBooks = searchResults.Where(b => !userBooks.Any(ub => ub.Id == b.Id)).ToList();

        titleSearchViewModel.Results = availableBooks;
        titleSearchViewModel.UserBooks = userBooks;

        return View(titleSearchViewModel);
    }
    
    // ログインしているユーザーが借りている書籍のリストを取得するプライベートメソッド
    private async Task<List<Book>> GetUserBooksAsync()
    {
        // ログインしていない場合は空のリストを返す
        if (!User.Identity.IsAuthenticated)
        {
            return new List<Book>();
        }

        // ログインしているユーザーを取得
        var currentUser = await _userManager.GetUserAsync(User);
        // ログインしていない場合は空のリストを返す
        if (currentUser == null)
        {
            return new List<Book>();
        }

        // ログインしているユーザーが借りている書籍のリストを取得
        return await _context.Books
            .Where(b => b.UserId == currentUser.Id && b.BorrowedStatus)
            .ToListAsync();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // GETアクション：メール作成フォームの表示
    public IActionResult CreateEMail()
    {
        return View(new EmailSendViewModel());
    }

    // POSTアクション：メール送信処理
    [HttpPost]
    public async Task<IActionResult> CreateEMail(EmailSendViewModel model)
    {
        // 入力されたデータがモデルのバリデーションルールに従っているかどうか検証する
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var successCount = 0; // 送信成功数
        var skippedCount = 0; // 送信スキップ数
        var errorCount = 0; // 送信失敗数
        var Messages = new List<string>();

        try
        {
            // ユーザー一覧を取得
            var users = await _userManager.Users.ToListAsync();

            // ユーザーごとにメール送信を行う
            foreach (var user in users)
            {
                // 社内メールアドレスかチェック
                if (!user.Email.EndsWith("@s-giken.co.jp", StringComparison.OrdinalIgnoreCase))
                {
                    skippedCount++;
                    continue;
                }

                try
                {
                    // メール送信
                    await _emailService.SendEmailAsync(
                        user.Email,
                        model.Subject,
                        model.Message
                    );
                    successCount++;
                    _logger.LogInformation($"送信成功：{user.Email}");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    var errorDetail = $"送信失敗：{user.Email}：{ex.Message}";
                    Messages.Add($"{user.Email}：{ex.Message}");
                    _logger.LogError(ex, $"送信エラー：{user.Email} - {ex.Message}");
                }
            }

            // 結果メッセージの設定
            var resultMessage = new StringBuilder();
            resultMessage.AppendLine($"送信完了：{successCount}件");
            if (skippedCount > 0)
            {
                resultMessage.AppendLine($"対象外（社外アドレス）：{skippedCount}件");
            }
            if (errorCount > 0)
            {
                resultMessage.AppendLine($"送信エラー：{errorCount}件");
                TempData["ErrorDetails"] = string.Join("<br>",Messages);
            }

            // 結果の設定
            TempData["Message"] = resultMessage.ToString();
            // Indexアクションにリダイレクト
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // 予期せぬエラーが発生した場合
            ModelState.AddModelError("", $"予期せぬエラーが発生しました：{ex.Message}");
            return View(model);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
