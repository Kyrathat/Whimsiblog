using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Whimsiblog.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogContext _db;

        public BlogController(BlogContext db)
        {
            _db = db;
        }

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
        public IActionResult Create()
        {
            return View(new Blog());
        }

        // POST: /Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Blog blog)
        {
            if (!ModelState.IsValid) return View(blog);

            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = blog.BlogId });
        }

        // GET: /Blog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.FindAsync(id.Value);
            if (blog == null) return NotFound();

            return View(blog);
        }

        // POST: /Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Name")] Blog blog)
        {
            if (id != blog.BlogId) return NotFound();
            if (!ModelState.IsValid) return View(blog);

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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.BlogId == id.Value);
            if (blog == null) return NotFound();

            return View(blog);
        }

        // POST: /Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog != null)
            {
                _db.Blogs.Remove(blog);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
