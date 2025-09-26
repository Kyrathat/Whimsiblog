using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace Whimsiblog.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogController(BlogContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string? CurrentUserId() => _userManager.GetUserId(User);
        private bool IsOwner(Blog b) => !string.IsNullOrEmpty(b.PrimaryOwnerUserId)
                             && b.PrimaryOwnerUserId == CurrentUserId();

        // GET: /Blog
        public async Task<IActionResult> Index()
        {
            var blogs = await _db.Blogs
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .ToListAsync();

            return View(blogs);
        }

        // GET: /Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.BlogId == id.Value);
            if (blog == null) return NotFound();

            return View(blog);
        }

        // GET: /Blog/Create
        [Authorize]
        public IActionResult Create()
        {
            return View(new Blog());
        }

        // POST: /Blog/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Blog blog)
        {
            if (!ModelState.IsValid) return View(blog);

            // Stamp ownership and audit
            blog.PrimaryOwnerUserId = CurrentUserId();
            blog.PrimaryOwnerUserName = User.Identity?.Name;
            blog.CreatedUtc = DateTime.UtcNow;

            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = blog.BlogId });
        }

        // GET: /Blog/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.FindAsync(id.Value);
            if (blog == null) return NotFound();

            if (!IsOwner(blog)) return Forbid(); // Owner check to make sure no one else can edit

            return View(blog);
        }

        // POST: /Blog/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Name")] Blog blog)
        {
            if (id != blog.BlogId) return NotFound();
            if (!ModelState.IsValid) return View(blog);

            // Load the tracked entity and verify ownership against the stored row
            var existing = await _db.Blogs.AsTracking().FirstOrDefaultAsync(b => b.BlogId == id);
            if (existing == null) return NotFound();
            if (!IsOwner(existing)) return Forbid(); // Another owner-check

            // Update allowed fields, TODO: Add tags
            existing.Name = blog.Name;

            try
            {
                _db.Update(blog);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await _db.Blogs.AnyAsync(b => b.BlogId == blog.BlogId);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Details), new { id = blog.BlogId });
        }

        // GET: /Blog/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.BlogId == id.Value);
            if (blog == null) return NotFound();

            if (!IsOwner(blog)) return Forbid(); // Another owner-check

            return View(blog);
        }

        // POST: /Blog/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog != null)
            {
                if (!IsOwner(blog)) return Forbid(); // Final (for now) owner-check
                _db.Blogs.Remove(blog);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
