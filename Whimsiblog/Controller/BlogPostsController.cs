using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [Authorize(Policy = "Age18+")]
        public async Task<IActionResult> Create()

        {
            ViewBag.AllTags = await _context.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Age18+")]
        public async Task<IActionResult> Create(BlogPost blogPost, int[]? SelectedTagIDs)
        {
            // --- Profanity checks ---
            if (_filter.ContainsProfanity(blogPost.Title ?? string.Empty))
                ModelState.AddModelError(nameof(BlogPost.Title), "Please remove profanity from the title.");

            if (_filter.ContainsProfanity(blogPost.Body ?? string.Empty))
                ModelState.AddModelError(nameof(BlogPost.Body), "Please remove profanity from the body.");

            // If anything failed validation, redisplay with tag list
            if (!ModelState.IsValid)
            {
                ViewBag.AllTags = await _context.Tags.ToListAsync();
                return View(blogPost);
            }

            // --- Figure out current user ---
            var userId = User.FindFirst("oid")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Challenge(); // not signed in or cannot identify

            blogPost.OwnerUserId = userId;
            blogPost.OwnerUserName = User.Identity?.Name;

            // --- Attach selected tags (many-to-many) ---
            if (SelectedTagIDs is { Length: > 0 })
            {
                var selectedTags = await _context.Tags
                    .Where(t => SelectedTagIDs.Contains(t.TagID))
                    .ToListAsync();

                blogPost.Tags = selectedTags; // EF will create the join rows
            }

            // --- Persist ---
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null) return NotFound();

            var blogPost = await _context.BlogPosts
                .Include(p => p.Tags) // Load existing tags
                .FirstOrDefaultAsync(p => p.BlogPostID == id);

            if (blogPost == null) return NotFound();

            ViewBag.AllTags = await _context.Tags.ToListAsync(); // for the multiselect
            ViewBag.SelectedTagIDs = blogPost.Tags.Select(t => t.TagID).ToArray(); // Preselected

            return View(blogPost);
        }

            // POST: BlogPosts/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("BlogPostID,Title,Body")] BlogPost blogPost, int[]? SelectedTagIDs)
        {
                if (id != blogPost.BlogPostID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {

                        if (_filter.ContainsProfanity(blogPost.Title) || _filter.ContainsProfanity(blogPost.Body))

                            if (!_filter.ContainsProfanity(blogPost.Title) && !_filter.ContainsProfanity(blogPost.Body))
                            {
                                _context.Update(blogPost);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                throw new Exception();
                            }


                        // Load the existing entity so we don't mess up the other database items
                        var entity = await _context.BlogPosts.FindAsync(id);
                        if (entity == null) return NotFound();

                        entity.Title = blogPost.Title;
                        entity.Body = blogPost.Body;
                        entity.UpdatedUtc = DateTime.UtcNow; // Used to update the Profile History

                    await _context.Entry(entity).Collection(e => e.Tags).LoadAsync();
                    entity.Tags.Clear();

                    if (SelectedTagIDs is { Length: > 0 })
                    {
                        var selectedTags = await _context.Tags
                            .Where(t => SelectedTagIDs.Contains(t.TagID))
                            .ToListAsync();

                        foreach (var t in selectedTags)
                            entity.Tags.Add(t);
                    }

                    await _context.SaveChangesAsync();

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
                // On redisplay, repopulate the tags
                ViewBag.AllTags = await _context.Tags.ToListAsync();
                ViewBag.SelectedTagIDs = SelectedTagIDs ?? Array.Empty<int>();
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
