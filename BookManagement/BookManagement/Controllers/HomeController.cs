using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    // �f�[�^�x�[�X�Ƃ̂��Ƃ��S������DbContext�N���X��p��
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        // �R���X�g���N�^��DbContext�N���X���󂯎��
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

    // Confirm�A�N�V���� GET��
    // �����Ƃ��ĊY������Book��id���󂯎��
    public IActionResult Confirm(int? id)
    {
        // id��null�̏ꍇ�̓G���[��Ԃ�
        if (id == null)
        {
            return NotFound();
        }

        // id�ɊY������Book���擾
        var book = _context.Books.FirstOrDefault(x => x.Id == id);

        // �r���[��Book��n���ĕ\��
        return View(book);
    }

    // Confirm�A�N�V���� POST��
    // �����Ƃ��ĊY������Book��id���󂯎��
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirm(int id)
    {
        // id�ɊY������Book���擾
        var book = _context.Books.FirstOrDefault(x => x.Id == id);

        // Book��BorrowedStatus��true�ɕύX
        book.BorrowedStatus = true;

        // Index�A�N�V�����Ƀ��_�C���N�g
        return RedirectToAction("Index");
    }
}
