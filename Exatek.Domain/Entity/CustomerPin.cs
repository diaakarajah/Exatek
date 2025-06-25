using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Entity
{
    public class CustomerPin : BaseEntity
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [MaxLength(6)] 
        public string PIN { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
