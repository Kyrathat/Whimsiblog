using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Whimsiblog.Controllers
{
    public class BlogCommentsController : Controller
    {
        private readonly BlogContext _context;

        public BlogCommentsController(BlogContext context)
        {
            _context = context;
        }

        // Helper to get current user's ID from claims
        private string? CurrentUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        }

        // Helper to check if current user is the owner of the comment
        private bool IsOwner(BlogComment c) =>
            !string.IsNullOrEmpty(c.OwnerUserId) &&
            c.OwnerUserId == CurrentUserId();

        // GET: BlogComments
        public async Task<IActionResult> Index()
        {
            var comments = await _context.BlogComments
                .Include(c => c.BlogPost)
                .Include(c => c.ParentComment)
                .Include(c => c.Replies) // first-level replies
                .AsNoTracking()
                .ToListAsync();

            return View(comments);
        }

        // GET: BlogComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var comment = await _context.BlogComments
                .Include(c => c.BlogPost)
                .Include(c => c.ParentComment)
                .Include(c => c.Replies) // load first-level replies
                    .ThenInclude(r => r.Replies) // optional: load second-level replies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.BlogCommentID == id);

            if (comment == null) return NotFound();

            return View(comment);
        }

        // GET: BlogComments/Create
        public IActionResult Create(int? blogPostId, int? parentCommentId)
        {
            ViewBag.BlogPostID = new SelectList(_context.BlogPosts, "BlogPostID", "Title", blogPostId);

            var comment = new BlogComment
            {
                BlogPostID = blogPostId ?? 0,
                ParentCommentID = parentCommentId
            };

            return View(comment);
        }

        // POST: BlogComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Body,BlogPostID,ParentCommentID")] BlogComment comment)
        {
            if (!ModelState.IsValid) return View(comment);

            comment.OwnerUserId = CurrentUserId();
            comment.OwnerUserName = User.Identity?.Name;

            _context.BlogComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: BlogComments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var comment = await _context.BlogComments.FindAsync(id.Value);
            if (comment == null) return NotFound();
            if (!IsOwner(comment)) return Forbid();

            return View(comment);
        }

        // POST: BlogComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogCommentID,Body")] BlogComment comment)
        {
            if (id != comment.BlogCommentID) return NotFound();

            var existing = await _context.BlogComments.FirstOrDefaultAsync(c => c.BlogCommentID == id);
            if (existing == null) return NotFound();
            if (!IsOwner(existing)) return Forbid();

            existing.Body = comment.Body;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: BlogComments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var comment = await _context.BlogComments
                .AsNoTracking()
                .Include(c => c.BlogPost)
                .Include(c => c.ParentComment)
                .FirstOrDefaultAsync(c => c.BlogCommentID == id.Value);

            if (comment == null) return NotFound();
            if (!IsOwner(comment)) return Forbid();

            return View(comment);
        }

        // POST: BlogComments/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.BlogComments.FindAsync(id);
            if (comment != null)
            {
                if (!IsOwner(comment)) return Forbid();
                _context.BlogComments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BlogCommentExists(int id)
        {
            return _context.BlogComments.Any(e => e.BlogCommentID == id);
        }
    }
}
