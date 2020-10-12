using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Good
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoodId { set; get; }
        [Required]
        public string GoodName { set; get; }
        [Required]
        public string BrandName { set; get; }
        public string UserAccountId { set; get; }
        public UserAccount UserAccount { set; get; }
   
    }
}
