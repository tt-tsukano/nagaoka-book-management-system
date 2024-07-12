using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Confirm/5
        [Authorize]
        public async Task<IActionResult> Confirm(int? id)
        {
            // idがnullの場合はNotFoundを返す
            if (id == null)
            {
                return NotFound();
            }

            // idに該当するBookを取得
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);

            // Bookがnullの場合はNotFoundを返す
            if (book == null)
            {
                return NotFound();
            }

            // BookをViewに渡して表示
            return View(book);
        }

        // POST: Books/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            // idに該当するBookを取得
            using (var transaction = _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // idに該当するBookを取得
                    var book = await _context.Books
                        // SQL文を実行
                        .FromSqlRaw("SELECT * FROM Books WHERE Id = {0}", id)
                        // 結果を取得
                        .FirstOrDefaultAsync();

                    // Bookがnullの場合はNotFoundを返す
                    if (book == null)
                    {
                        return NotFound();
                    }

                    // BookのBorrowedStatusをtrueに変更
                    book.BorrowedStatus = true;
                    book.BorrowedDate = DateTime.Now;
                    book.ReturnDate = book.BorrowedDate.HasValue ? DateTime.Now.AddDays(14) : (DateTime?)null;
                    book.UserId = int.Parse(_userManager.GetUserId(User));

                    // Bookを更新
                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    // コミット
                    await transaction.Result.CommitAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    // ロールバック
                    await transaction.Result.RollbackAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        // GET: Books/Return/5
        [Authorize]
        public async Task<IActionResult> Return(int? id)
        {
            // idがnullの場合はNotFoundを返す
            if (id == null)
            {
                return NotFound();
            }

            // idに該当するBookを取得
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);

            // Bookがnullの場合はNotFoundを返す
            if (book == null)
            {
                return NotFound();
            }

            // BookをViewに渡して表示
            return View(book);
        }

        // POST: Books/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Return(int id)
        {
            // idに該当するBookを取得
            var book = await _context.Books.FindAsync(id);

            // Bookがnullの場合はNotFoundを返す
            if (book == null)
            {
                return NotFound();
            }

            // BookのBorrowedStatusをfalseに変更
            book.BorrowedStatus = false;

            // BookのBorrowedDateをnullに設定
            book.BorrowedDate = null;

            // BookのReturnDateをnullに設定
            book.ReturnDate = null;

            // BookのUserIdを0に設定
            book.UserId = null;

            // Bookを更新
            _context.Update(book);

            // 保存
            await _context.SaveChangesAsync();

            // 書籍一覧画面にリダイレクト
            return RedirectToAction("Index","Home");
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,BorrowedStatus,BorrowedDate,ReturnDate,PublishedYear,UserId,Author")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                // 書籍一覧画面にリダイレクト
                return RedirectToAction("Index", "Home");
            }
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,BorrowedStatus,BorrowedDate,ReturnDate,PublishedYear,UserId,Author")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                // 論理削除のためにIsDeletedをtrueに変更
                book.IsDeleted = true;
                // 更新
                _context.Update(book);
                // 保存
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
