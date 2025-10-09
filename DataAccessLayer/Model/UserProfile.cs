using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    // PK == Azure AD user id (oid/nameidentifier). We store birthdate to age-gate.
    public class UserProfile
    {
        public string Id { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }// NULL until user sets it
        public AvatarType Avatar { get; set; } = AvatarType.DoomsdayApparatus;
        //public string? Bio { get; set; } optional short bio
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
