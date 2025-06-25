using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Dtos
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
