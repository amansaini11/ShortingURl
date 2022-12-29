using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortingURLTest.Web.Entity
{
    public class AppUser : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NickName { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;    
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}

