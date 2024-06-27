using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;

namespace BookManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Confirm/5
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
        public async Task<IActionResult> Confirm(int id)
        {
            // idに該当するBookを取得
            var book = await _context.Books.FindAsync(id);

            // Bookがnullの場合はNotFoundを返す
            if (book == null)
            {
                return NotFound();
            }

            // BookのBorrowedStatusをtrueに変更
            book.BorrowedStatus = true;

            // BookのBorrowedDateに現在日時を設定
            book.BorrowedDate = DateTime.Now;

            // BookのReturnDateにBorrowedDateから2週間後の日時を設定
            book.ReturnDate = book.BorrowedDate.AddDays(14);

            // Bookを更新
            _context.Update(book);

            // 保存
            await _context.SaveChangesAsync();

            // Indexにリダイレクト（暫定処理）
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Return/5
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

            // BookのBorrowedDateをnullに設定したい
            book.BorrowedDate = DateTime.MinValue;

            // BookのReturnDateをnullに設定したい
            book.ReturnDate = DateTime.MinValue;

            // Bookを更新
            _context.Update(book);

            // 保存
            await _context.SaveChangesAsync();

            // Indexにリダイレクト（暫定処理）
            return RedirectToAction(nameof(Index));
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,BorrowedStatus,BorrowedDate,ReturnDate,PublishedYear,UserId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,BorrowedStatus,BorrowedDate,ReturnDate,PublishedYear,UserId")] Book book)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
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
