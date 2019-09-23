using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Entities
{
    [Table("user")]
    public class User 
    {
        [Column("ID")]
        public int ID { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; } 

        [Column("LastName")]
        public string LastName { get; set; } 

        [Column("UserName")]
        public string UserName { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("PasswordHash")]
        public byte[] PasswordHash { get; set; }

        [Column("PasswordSalt")]
        public byte[] PasswordSalt { get; set; }

        [Column("Password")]
        public string Password { get; set; }

        [Column("userId")]
        public string userId { get; set; }

    }
}
