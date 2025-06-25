using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Entity
{
    public class CustomerToken : BaseEntity
    {

        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string Channel { get; set; } // 'email_verification' or 'phone_verification'

        [StringLength(255)]
        public string Token { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime ExpiredAt { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Customer Customer { get; set; }
    }
}
