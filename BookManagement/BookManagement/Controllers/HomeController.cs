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

    // �R���X�g���N�^�ł̈ˑ�������
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager, IEmailService emailService)
    {
        // ���O�o�͋@�\
        _logger = logger;
        // ���[�U�[�Ǘ��@�\
        _userManager = userManager;
        // ���[�����M�@�\
        _emailService = emailService;
        // �f�[�^�x�[�X�R���e�L�X�g
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        try
        {
            // �g�����U�N�V�����X�R�[�v���g�p
            using var transaction = await _context.Database.BeginTransactionAsync();

            var userBooks = await GetUserBooksAsync();

            // ToListAsync���g�p���Ė����I�ɃN�G�������s
            var allBooks = await _context.Books
                .AsNoTracking() // �g���b�L���O���Ȃ�
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

            // ���[�U�[���؂�Ă��鏑�Ђ����O
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
            _logger.LogError(ex, "�G���[���������܂���");
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

        // ���[�U�[���؂�Ă��鏑�Ђ����O
        var availableBooks = searchResults.Where(b => !userBooks.Any(ub => ub.Id == b.Id)).ToList();

        titleSearchViewModel.Results = availableBooks;
        titleSearchViewModel.UserBooks = userBooks;

        return View(titleSearchViewModel);
    }
    
    // ���O�C�����Ă��郆�[�U�[���؂�Ă��鏑�Ђ̃��X�g���擾����v���C�x�[�g���\�b�h
    private async Task<List<Book>> GetUserBooksAsync()
    {
        // ���O�C�����Ă��Ȃ��ꍇ�͋�̃��X�g��Ԃ�
        if (!User.Identity.IsAuthenticated)
        {
            return new List<Book>();
        }

        // ���O�C�����Ă��郆�[�U�[���擾
        var currentUser = await _userManager.GetUserAsync(User);
        // ���O�C�����Ă��Ȃ��ꍇ�͋�̃��X�g��Ԃ�
        if (currentUser == null)
        {
            return new List<Book>();
        }

        // ���O�C�����Ă��郆�[�U�[���؂�Ă��鏑�Ђ̃��X�g���擾
        return await _context.Books
            .Where(b => b.UserId == currentUser.Id && b.BorrowedStatus)
            .ToListAsync();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // GET�A�N�V�����F���[���쐬�t�H�[���̕\��
    public IActionResult CreateEMail()
    {
        return View(new EmailSendViewModel());
    }

    // POST�A�N�V�����F���[�����M����
    [HttpPost]
    public async Task<IActionResult> CreateEMail(EmailSendViewModel model)
    {
        // ���͂��ꂽ�f�[�^�����f���̃o���f�[�V�������[���ɏ]���Ă��邩�ǂ������؂���
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var successCount = 0; // ���M������
        var skippedCount = 0; // ���M�X�L�b�v��
        var errorCount = 0; // ���M���s��
        var Messages = new List<string>();

        try
        {
            // ���[�U�[�ꗗ���擾
            var users = await _userManager.Users.ToListAsync();

            // ���[�U�[���ƂɃ��[�����M���s��
            foreach (var user in users)
            {
                // �Г����[���A�h���X���`�F�b�N
                if (!user.Email.EndsWith("@s-giken.co.jp", StringComparison.OrdinalIgnoreCase))
                {
                    skippedCount++;
                    continue;
                }

                try
                {
                    // ���[�����M
                    await _emailService.SendEmailAsync(
                        user.Email,
                        model.Subject,
                        model.Message
                    );
                    successCount++;
                    _logger.LogInformation($"���M�����F{user.Email}");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    var errorDetail = $"���M���s�F{user.Email}�F{ex.Message}";
                    Messages.Add($"{user.Email}�F{ex.Message}");
                    _logger.LogError(ex, $"���M�G���[�F{user.Email} - {ex.Message}");
                }
            }

            // ���ʃ��b�Z�[�W�̐ݒ�
            var resultMessage = new StringBuilder();
            resultMessage.AppendLine($"���M�����F{successCount}��");
            if (skippedCount > 0)
            {
                resultMessage.AppendLine($"�ΏۊO�i�ЊO�A�h���X�j�F{skippedCount}��");
            }
            if (errorCount > 0)
            {
                resultMessage.AppendLine($"���M�G���[�F{errorCount}��");
                TempData["ErrorDetails"] = string.Join("<br>",Messages);
            }

            // ���ʂ̐ݒ�
            TempData["Message"] = resultMessage.ToString();
            // Index�A�N�V�����Ƀ��_�C���N�g
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // �\�����ʃG���[�����������ꍇ
            ModelState.AddModelError("", $"�\�����ʃG���[���������܂����F{ex.Message}");
            return View(model);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
