using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Dtos
{
    public class VerifyPhoneRequest
    {
        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string Token { get; set; }
    }
}
