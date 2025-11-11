using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;

namespace Whimsiblog.Controllers
{
    public class TagsController : Controller
    {
        private readonly BlogContext _context;
        private ProfanityFilter.ProfanityFilter _filter = new ProfanityFilter.ProfanityFilter();

        public TagsController(BlogContext context)
        {
            _context = context;
        }

        // A helper for the profanity
        private bool HasProfanity(string? text)
        {
            var trimmed = text.Trim();

            // Use the library's main detection API
            var hits = _filter.DetectAllProfanities(trimmed);

            var result = hits != null && hits.Count > 0;

            Console.WriteLine($"[ProfanityCheck] \"{trimmed}\" => {result}"); // Debugging helper
            return result;
        }


        // GET: Tags
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tags.ToListAsync());
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.TagID == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagID,Name")] Tag tag)
        {
            // If failed, show form again
            if (!ModelState.IsValid)
            {
                return View(tag);
            }

            // Normalize once
            tag.Name = (tag.Name ?? string.Empty).Trim();

            // Profanity check
            if (HasProfanity(tag.Name))
            {
                ModelState.AddModelError(nameof(Tag.Name), "Please remove profanity.");
            }

            // Then duplicate name check
            if (await TagNameExistsAsync(tag.Name))
            {
                ModelState.AddModelError(nameof(Tag.Name), "A tag with this name already exists.");
            }

            // Finally, save
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagID,Name")] Tag tag)
        {
            if (id != tag.TagID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await TagNameExistsAsync(tag.Name, tag.TagID)) // tag.TagID is used here as a security check. It's checking
                {                                                        // if the tag being edited the same one the URL says it is.
                    ModelState.AddModelError("Name", "A tag with this name already exists.");
                    return View(tag);
                }

                try
                {
                    if (!_filter.ContainsProfanity(tag.Name))
                    {
                        _context.Update(tag);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagID))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.TagID == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagID == id);
        }

        private async Task<bool> TagNameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Tags
                .AnyAsync(t => t.Name.ToLower() == name.ToLower() && (excludeId == null || t.TagID != excludeId));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<string>());
            }

            query = query.Trim().ToLower(); // normalize the input

            var matches = await _context.Tags
                .Where(t => t.Name.ToLower().StartsWith(query)) // normalize the DB field
                .Select(t => t.Name)
                .ToListAsync();

            return Json(matches);
        }

        [HttpPost("/tags/add")]
        public IActionResult AddTag([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Tag name is required.");

            var normalized = name.Trim();

            // profanity check
            if (HasProfanity(normalized))
                return BadRequest("Profanity is not allowed in tag names.");

            // duplicate check
            var existing = _context.Tags
                .FirstOrDefault(t => t.Name.ToLower() == normalized.ToLower());

            if (existing != null)
                return Conflict("Tag already exists.");

            // Save
            var newTag = new Tag { Name = normalized };
            _context.Tags.Add(newTag);
            _context.SaveChanges();

            return Json(new { TagID = newTag.TagID, Name = newTag.Name });
        }

    }
}
