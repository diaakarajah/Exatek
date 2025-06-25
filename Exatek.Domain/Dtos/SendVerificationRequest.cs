using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Dtos
{
    public class SendVerificationRequest
    {
        [Required]
        public string Channel { get; set; } // "phone" or "email"
    }
}
