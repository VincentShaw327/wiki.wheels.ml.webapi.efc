using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("t_wiki_category")]
    public class Category
    {
        [Column("uObjectUUID")]
        public int ID { get; set; }

        //[Column("uObjectUUID")]
        //public int uObjectUUID { get; set; }

        [Column("uOwnerUUID")]
        public int uOwnerUUID { get; set; }

        [Column("strImage")]
        public string strImage { get; set; }

        [Column("strTitle")]
        public string strTitle { get; set; }

        [Column("nFlag")]
        public int nFlag { get; set; }

        [Column("nDelFlag")]
        public int nDelFlag { get; set; }
    }
}
