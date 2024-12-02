using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace BookManagement.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // 設定ファイル(appsettings.json)からSMTPサーバーの設定を取得
                var smtpHost = _configuration["Email:Host"];
                var smtpPort = int.Parse(_configuration["Email:Port"]);
                var fromAddress = _configuration["Email:From"];
                var username = _configuration["Email:UserName"];
                var password = _configuration["Email:Password"];

                // SMTPサーバーの設定をログ出力
                _logger.LogInformation($"SMTPサーバーへの接続を試みています: {_configuration["Email:Host"]}：{_configuration["Email:Port"]}");

                // 設定値の検証
                if (string.IsNullOrEmpty(fromAddress))
                {
                    throw new InvalidOperationException("送信元メールアドレスが設定されていません");
                }

                using (var client = new SmtpClient())
                {
                    //クライアントの設定
                    client.Host = _configuration["Email:Host"];
                    client.Port = 25;
                    client.EnableSsl = false;
                    client.UseDefaultCredentials = false;

                    var credential = new NetworkCredential(username, password);
                    client.Credentials = credential;

                    // タイムアウト設定の追加
                    client.Timeout = 30000; // 30秒

                    // メッセージの作成
                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.From = new MailAddress(fromAddress); // 送信元メールアドレス
                        emailMessage.To.Add(new MailAddress(email)); // 送信先メールアドレス
                        emailMessage.Subject = subject; // 件名
                        emailMessage.Body = message; // 本文
                        emailMessage.IsBodyHtml = true; // HTML形式

                        // ログ出力
                        _logger.LogInformation($"メール送信中: {email}");

                        // SMTPサーバーの設定をログ出力
                        _logger.LogInformation($"SMTP設定：Host={client.Host}, Port={client.Port}, SSL={client.EnableSsl}, UseDefaultCredentials={client.UseDefaultCredentials}");

                        // メール送信
                        await client.SendMailAsync(emailMessage);

                        _logger.LogInformation($"メール送信完了: {email}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"メール送信エラー: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"詳細: {ex.InnerException.Message}");
                }
                throw;
            }

        }
    }
}
