using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;

namespace Whimsiblog.Controllers
{
    // Only authenticated users can view/manage their profile.
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly BlogContext _db;
        public ProfileController(BlogContext db) => _db = db;

        // Pull the stable Azure AD user id
        private string? CurrentAadId() =>
            User.FindFirst("oid")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = CurrentAadId();
            if (id is null) return Challenge();

            var profile = await _db.UserProfiles.AsNoTracking()
                              .FirstOrDefaultAsync(p => p.Id == id);

            // If the user doesn't have a profile yet, redirect them to create one
            if (profile is null) return RedirectToAction(nameof(Edit));

            int? age = null;
            if (profile.BirthDate is DateTime dob)
            {
                var today = DateTime.UtcNow.Date;
                var years = today.Year - dob.Year;
                if (dob.Date > today.AddYears(-years)) years--;
                age = years;
            }

            // Recent activity
            ViewBag.RecentPosts = await _db.BlogPosts.AsNoTracking()
                .Where(p => p.OwnerUserId == id) // Awaiting new field
                .OrderByDescending(p => p.BlogPostID)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentComments = await _db.BlogComments.AsNoTracking()
                .Where(c => c.OwnerUserId == id)
                .OrderByDescending(c => c.BlogCommentID)
                .Include(c => c.BlogPost)
                .Take(5)
                .ToListAsync();

            ViewBag.PostCount = await _db.BlogPosts.AsNoTracking().CountAsync(p => p.OwnerUserId == id);  // Awaiting new field
            ViewBag.CommentCount = await _db.BlogComments.AsNoTracking().CountAsync(c => c.OwnerUserId == id);

            ViewBag.Age = age;
            ViewBag.IsAdult = age is >= 18;
            ViewBag.IsOwner = true;

            // Pass the entity as the model
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var id = CurrentAadId();
            if (id is null) return Challenge();

            // Find by PK
            var profile = await _db.UserProfiles.FindAsync(id)
                         ?? new UserProfile
                         {
                             Id = id,
                             DisplayName = User.Identity?.Name,
                             Email = User.FindFirst(ClaimTypes.Email)?.Value
                         };

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfile input)
        {
            var id = CurrentAadId();
            if (id is null) return Challenge(); // not signed in
            if (id != input.Id) return Forbid(); // cannot edit someone else

            // Basic validation
            if (input.BirthDate is { } dob && dob > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(input.BirthDate), "Birth date cannot be in the future.");

            if (!ModelState.IsValid)
                return View(input); // redisplay with messages

            var exists = await _db.UserProfiles.AsNoTracking().AnyAsync(p => p.Id == id);

            input.UpdatedUtc = DateTime.UtcNow;
            if (!exists)
            {
                input.CreatedUtc = DateTime.UtcNow;
                _db.UserProfiles.Add(input);
            }
            else
            {
                _db.UserProfiles.Update(input);
            }

            await _db.SaveChangesAsync();

            // User feedback shown after redirect
            TempData["ProfileSaved"] = "Your profile was saved.";

            // Avoid resubmits on refresh
            return RedirectToAction(nameof(Edit));
        }

        
    }
}
