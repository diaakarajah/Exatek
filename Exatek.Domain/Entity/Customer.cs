using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Entity
{
    public class Customer : BaseEntity
    {
        [Required]
        public string CustomerName { get; set; }
        [Required]
        [MaxLength(50)]
        public string ICNumber { get; set; }
        [Required]
        [MaxLength(50)]
        public string MobileNumber { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;

        public bool IsPhoneConfirmed { get; set; } = false;

        public bool IsTermsAccepted { get; set; } = false;


        // Navigation property
        public virtual ICollection<CustomerToken> CustomerTokens { get; set; } = new List<CustomerToken>();
        public virtual CustomerPin CustomerPin { get; set; }
    }
}
