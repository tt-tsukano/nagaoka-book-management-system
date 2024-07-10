using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using BookManagement.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BookManagement.ViewModels;
using System.Text;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

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
