using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;

namespace Whimsiblog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly BlogContext _context;
        private ProfanityFilter.ProfanityFilter _filter = new ProfanityFilter.ProfanityFilter();

        public BlogPostsController(BlogContext context)
        {
            _context = context;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogPosts.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.BlogPostID == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.AllTags = await _context.Tags.ToListAsync();
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost blogPost, int[] SelectedTagIDs)
        {
            if (ModelState.IsValid)
            {
                // Load tags from DB for the selected IDs
                if (SelectedTagIDs != null && SelectedTagIDs.Length > 0)
                {
                    var selectedTags = await _context.Tags
                        .Where(t => SelectedTagIDs.Contains(t.TagID))
                        .ToListAsync();

                    blogPost.Tags = selectedTags;
                }

                // Add the blog post to the database
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag.AllTags in case of redisplay
            ViewBag.AllTags = await _context.Tags.ToListAsync();
            return View(blogPost);
        }


        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogPostID,Title,Body")] BlogPost blogPost)
        {
            if (id != blogPost.BlogPostID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_filter.ContainsProfanity(blogPost.Title) && !_filter.ContainsProfanity(blogPost.Body))
                    {
                        _context.Update(blogPost);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.BlogPostID))
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
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.BlogPostID == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.BlogPostID == id);
        }
    }
}
