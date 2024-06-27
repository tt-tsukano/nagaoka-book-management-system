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

    // �p�X���[�h���n�b�V�������郁�\�b�h
    public string HashPassword(string password)
    {
        // SHA256�̃n�b�V���l���v�Z����N���X���쐬
        // SHA256�͓��͂��ꂽ�f�[�^����n�b�V���l�𐶐�����A���S���Y���̈��
        // using�u���b�N���g�����ƂŁA�������I�������玩���I�Ƀ��\�[�X���������
        using (SHA256 sha256 = SHA256.Create())
        {
            // �p�X���[�h���o�C�g�z��ɕϊ�
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // �n�b�V���v�Z
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            // �n�b�V���l�𕶎���ɕϊ����ĕԂ�
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

        //TitleSearchViewModel��Results�ɋL���ꗗ���i�[
        var viewModel = new TitleSearchViewModel
        {
            Results = bookslist
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
    //Login���\�b�h�̈�����Email��Password��ǉ�
    public async Task<IActionResult> Login(string Email, string Password)
    {
        //���[�U�[�����͂����p�X���[�h���n�b�V����
         Password = HashPassword(Password);

        //Email��Password����v����User���擾
        var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == Email);// && m.PasswordHash == Password);

        //User��null�̏ꍇ��NotFound��Ԃ�
        if (user == null)
        {
            return NotFound();
        }

        //User��Index��View�ɓn���ĕ\��
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
