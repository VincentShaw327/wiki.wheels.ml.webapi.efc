using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        //new public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //new public string UserName { get; set; }
        //new public string Email { get; set; }
        //new public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
    }
}
