using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("t_wiki_item")]
    public class Wiki
    {
        [Column("uObjectUUID")]
        public int ID { get; set; }

        [Column("uObjectParentUUID")]
        public int uObjectParentUUID { get; set; }

        [Column("uCategoryUUID")]
        public int uCategoryUUID { get; set; }

        [Column("uTopicUUID")]
        public int uTopicUUID { get; set; }

        [Column("orderID")]
        public int orderID { get; set; }

        [Column("strImage")]
        public string strImage { get; set; }

        [Column("strTitle")]
        public string strTitle { get; set; }

        [Column("strLabel")]
        public string strLabel { get; set; }

        [Column("strDesc")]
        public string strDesc { get; set; }

        [Column("strNote")]
        public string strNote { get; set; }

        [Column("strContent")]
        public string strContent { get; set; }

        [Column("uOwnerUUID")]
        public int uOwnerUUID { get; set; }

        [Column("nFlag")]
        public int nFlag { get; set; }

        [Column("uPublisherUUID")]
        public int uPublisherUUID { get; set; }

        [Column("nCore")]
        public int nCore { get; set; }

        [Column("nHasChildren")]
        public int nHasChildren { get; set; }

        [Column("nDelFlag")]
        public int nDelFlag { get; set; } = 1;

        [Column("dtCreate")]
        public DateTime dtCreate { get; set; }

        [Column("dtUpdate")]
        public DateTime dtUpdate { get; set; }



        public int CompareTo(Wiki comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
                return 1;

            else
                return this.orderID.CompareTo(comparePart.orderID);
        }
    }
}
