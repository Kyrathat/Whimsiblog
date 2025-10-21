using Microsoft.AspNetCore.Authorization;

namespace Whimsiblog.Helpers
{
    // Marker that means: user must be at least MinAge
    public class AgeRequirement : IAuthorizationRequirement
    {
        public int MinAge { get; }
        public AgeRequirement(int minAge) => MinAge = minAge;
    }
}
