using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Model;

namespace Whimsiblog.Controllers
{
    // Only authenticated users can view their profile.
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
        /*
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

            // Authorization check
            if (id != input.Id) return Forbid();

            // Basic server-side validation
            if (input.BirthDate is { } dob && dob > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(input.BirthDate), "Birth date cannot be in the future.");

            if (!ModelState.IsValid)
                return View(input); // redisplay the form with validation messages

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

            //User feedback shown after redirect
            TempData["ProfileSaved"] = "Your profile was saved.";

            // Avoids resubmits on refresh.
            return RedirectToAction(nameof(Edit));
        } */
    }
}
