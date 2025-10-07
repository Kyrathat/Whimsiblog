using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace Whimsiblog.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogContext _db;

        public BlogController(BlogContext db)
        {
            _db = db;
        }

        private string? CurrentUserId()
        {
            // Works for Azure AD / Microsoft.Identity.Web
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                // Azure AD object id claim (if the above is null)
                ?? User?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        }

        private bool IsOwner(Blog b) =>
            !string.IsNullOrEmpty(b.PrimaryOwnerUserId) &&
            b.PrimaryOwnerUserId == CurrentUserId();


        // GET: /Blog
        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Blogs.Include(b => b.Tags).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(b => b.Name.Contains(q));
            }

            var blogs = await query
                .OrderBy(b => b.Name)
                .ToListAsync();

            // pass the current query back to the view for the search box
            ViewData["q"] = q;
            return View(blogs);
        }

        // GET: /Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.Include(b => b.Tags).AsNoTracking().FirstOrDefaultAsync(b => b.BlogId == id.Value);
            if (blog == null) return NotFound();

            return View(blog);
        }

        // GET: /Blog/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Tags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
            return View(new Blog());
        }

        // POST: /Blog/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Blog blog, int[] selectedTags)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.Tags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
                return View(blog);
            }

            // Normalize input
            blog.Description = blog.Description?.Trim();

            // If empty, assign a random placeholder
            if (string.IsNullOrWhiteSpace(blog.Description))
            {
                blog.Description = DataAccessLayer.Helpers.BlogDescriptionPlaceholderText.RandomText();
            }

            blog.PrimaryOwnerUserId = CurrentUserId();
            blog.PrimaryOwnerUserName = User.Identity?.Name;
            blog.CreatedUtc = DateTime.UtcNow;

            // Add selected tags
            if (selectedTags != null && selectedTags.Length > 0)
            {
                var tags = await _db.Tags.Where(t => selectedTags.Contains(t.TagID)).ToListAsync();
                blog.Tags = tags;
            }

            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = blog.BlogId });
        }


        // GET: /Blog/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.Include(b => b.Tags).FirstOrDefaultAsync(b => b.BlogId == id.Value);
            if (blog == null) return NotFound();

            if (!IsOwner(blog)) return Forbid(); // Owner check to make sure no one else can edit

            ViewBag.Tags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
            return View(blog);
        }

        // POST: /Blog/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Name,Description")] Blog blog, int[] selectedTags)
        {
            if (id != blog.BlogId) return NotFound();
            if (!ModelState.IsValid) 
            {
                ViewBag.Tags = await _db.Tags.OrderBy(t => t.Name).ToListAsync();
                return View(blog);
            }

            var existing = await _db.Blogs.Include(b => b.Tags).FirstOrDefaultAsync(b => b.BlogId == id);
            if (existing == null) return NotFound();
            if (!IsOwner(existing)) return Forbid();

            existing.Name = blog.Name;
            existing.Description = blog.Description;

            // Update tags
            existing.Tags.Clear();
            if (selectedTags != null && selectedTags.Length > 0)
            {
                var tags = await _db.Tags.Where(t => selectedTags.Contains(t.TagID)).ToListAsync();
                foreach (var tag in tags)
                {
                    existing.Tags.Add(tag);
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = existing.BlogId });
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
