using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using BookManagement.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BookManagement.ViewModels;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var userBooks = await GetUserBooksAsync();
        var allBooks = await _context.Books.ToListAsync();
        // ���[�U�[���؂�Ă��鏑�Ђ����O
        var availableBooks = allBooks.Where(b => !userBooks.Any(ub => ub.Id == b.Id)).ToList();

        var viewModel = new TitleSearchViewModel
        {
            Results = availableBooks,
            UserBooks = userBooks
        };

        return View(viewModel);
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

        var searchResults = await books.ToListAsync();
        // ���[�U�[���؂�Ă��鏑�Ђ����O
        var availableBooks = searchResults.Where(b => !userBooks.Any(ub => ub.Id == b.Id)).ToList();

        titleSearchViewModel.Results = availableBooks;
        titleSearchViewModel.UserBooks = userBooks;

        return View(titleSearchViewModel);
    }
    /*public async Task<IActionResult> Index()
    {
        var bookslist = await _context.Books.ToListAsync();

        //TitleSearchViewModel��Results�ɋL���ꗗ���i�[
        //UserBooks�Ƀ��[�U�[���؂�Ă��鏑�Ђ̈ꗗ���i�[
        var viewModel = new TitleSearchViewModel
        {
            Results = bookslist,
            UserBooks = await GetUserBooksAsync()
        };

        //bookslist�ł͂Ȃ��AviewModel��view�ɕԂ�
        return View(viewModel);
    }


    //�����{�^������������̑���
    [HttpPost]
    public async Task<IActionResult> Index(TitleSearchViewModel titleSearchViewModel)
    {
        IQueryable<Book> books = _context.Books;

        //�^�C�g���̑S����v
        if (!string.IsNullOrEmpty(titleSearchViewModel.Title))
        {
            books = books.Where(b => b.Title.Contains(titleSearchViewModel.Title));
        }

        titleSearchViewModel.Results = await books.ToListAsync();
        titleSearchViewModel.UserBooks = await GetUserBooksAsync();

        return View(titleSearchViewModel);
    }*/

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
