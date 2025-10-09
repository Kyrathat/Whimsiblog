using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DataAccess;

namespace Whimsiblog.Helpers
{
    /// <summary>
    /// Authorization handler that verifies the current user meets a minimum age.
    /// We store a user profile (keyed by AAD id) with a BirthDate; this handler
    /// loads that profile and calculates age in UTC (date-only) to avoid time-of-day issues.
    /// </summary>
    
    // AAD is Azure Active Directory
    public class AgeRequirementHandler : AuthorizationHandler<AgeRequirement>
    {
        private readonly BlogContext _db;
        
        public AgeRequirementHandler(BlogContext db) => _db = db;

        /// <summary>
        /// Called by the authorization system when a resource requires AgeRequirement.
        /// If the user meets the requirement, call context.Succeed(requirement).
        /// Doing nothing == requirement not met (authorization will fail).
        /// </summary>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AgeRequirement requirement)
        {
            // Identify the current user
            string? aadId =
                (context.User.FindFirst("oid") ??
                 context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                 context.User.FindFirst("sub"))?.Value;

            // If we can't resolve an id, we can't evaluate age
            if (string.IsNullOrEmpty(aadId))
                return;
            
            // Load the user's profile
            var profile = await _db.UserProfiles
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(p => p.Id == aadId);

            if (profile?.BirthDate is null)
                return;
            
            //calculate age accurately
            var today = DateTime.UtcNow.Date;
            var dob = profile.BirthDate.Value.Date;

            int age = today.Year - dob.Year;
            if (today < dob.AddYears(age))
                age--;

            // If they are 18 or older, then the requirement was a succsess
            if (age >= requirement.MinAge)
                context.Succeed(requirement);

        }
    
    } 
    
}
