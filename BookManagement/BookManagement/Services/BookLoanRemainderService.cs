// Services/BookLoanReminderService.cs
using BookManagement.Data;
using BookManagement.Models;
using BookManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// バックグラウンドサービスを継承したクラス
public class BookLoanReminderService : BackgroundService
{
    // 依存性注入のためのフィールド
    private readonly ILogger<BookLoanReminderService> _logger; // ログを記録するためのインターフェース
    private readonly IServiceProvider _serviceProvider; // CheckOverdueBooksメソッド内でデータベースやメールを活用するためのインターフェース
    private readonly IConfiguration _configuration; // アプリケーションの設定情報を取得するためのインターフェース

    // コンストラクタ。クラス内の他のメソッドで依存性を利用できるようになる
    public BookLoanReminderService(
        ILogger<BookLoanReminderService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    // バックグランド処理を行うメソッド。バックグラウンドサービスをオーバーライドしている
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) // 引数はサービスの停止要求を受け取るためのトークン
    {
        // バックグラウンドサービスの停止要求がなければ処理が走る
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 期限切れ本のチェック処理を実行
                await CheckOverdueBooks();
                // 24時間待機
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "貸出期限チェック処理でエラーが発生しました");
                // エラー発生時は1時間待機
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

    // 期限切れの本をチェックするメソッド
    private async Task CheckOverdueBooks()
    {
        // スコープを作成。処理完了後にリソースを開放するため
        using var scope = _serviceProvider.CreateScope();
        // スコープ内でサービスを取得
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        // 貸出期限が過ぎている本を検索
        var overdueBooks = await dbContext.Books
            .Include(b => b.User)
            .Where(b =>
                b.BorrowedStatus &&
                b.ReturnDate.HasValue &&
                b.ReturnDate.Value < DateTime.Now &&
                !b.IsDeleted &&
                b.User != null)
            .ToListAsync();

        // 各本についてのメールを送信
        foreach (var book in overdueBooks)
        {
            var daysOverdue = (DateTime.Now - book.ReturnDate.Value).Days;
            var emailSubject = "図書返却期限超過のお知らせ";
            var emailBody = $@"
                <h2>図書返却期限超過のお知らせ</h2>
                <p>{book.User.UserName} さん</p>
                <p>以下の図書の返却期限が{daysOverdue}日超過しています。</p>
                <ul>
                    <li>タイトル：{book.Title}</li>
                    <li>著者：{book.Author}</li>
                    <li>返却期限：{book.ReturnDate:yyyy/MM/dd}</li>
                </ul>
                <p>お早めにご返却をお願いいたします。</p>";

            try
            {
                await emailService.SendEmailAsync(book.User.Email, emailSubject, emailBody);
                _logger.LogInformation($"リマインダーメール送信完了: BookId={book.Id}, User={book.User.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"リマインダーメール送信失敗: BookId={book.Id}, User={book.User.Email}");
            }
        }
    }
}