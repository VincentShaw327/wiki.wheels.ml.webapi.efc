using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class AccountRole : IdentityUserRole<int>
    {
        [Key]
        public int ClubId { get; set; }
        //public override int RoleId
        //{
        //    get;
        //    set;
        //}
        //    public int TKey UserId
        //{
        //    get;
        //    set;
    }
    //public IdentityUserRole()
    //{
    //}

    //public string somenewproperty()
    //{
    //    get; set;
    //}
}

