using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class User
    {
        public int UserID { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }

        public AvatarType Avatar { get; set; } = AvatarType.ProfessionalNoisemaker;

        //public string? Bio { get; set; }
    }
}
