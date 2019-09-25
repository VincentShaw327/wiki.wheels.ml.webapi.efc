using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("t_wiki_topic")]
    public class Topic
    {
        [Column("uObjectUUID")]
        public int ID { get; set; }

        //[Column("uObjectUUID")]
        //public int uObjectUUID { get; set; }

        [Column("uObjectCategoryUUID")]
        public int uObjectCategoryUUID { get; set; }

        [Column("strImage")]
        public string strImage { get; set; }

        [Column("strID")]
        public string strID { get; set; }

        [Column("strName")]
        public string strName { get; set; }

        [Column("strNote")]
        public string strNote { get; set; }

        [Column("strTempUser")]
        public string strTempUser { get; set; }

        [Column("uCreatorUUID")]
        public int uCreatorUUID { get; set; }

        [Column("uDutyUserUUID")]
        public int uDutyUserUUID { get; set; }

        [Column("nDelFlag")]
        public int nDelFlag { get; set; }
    }
}
