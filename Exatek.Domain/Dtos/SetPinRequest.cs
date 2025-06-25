using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.Domain.Dtos
{
    public class SetPinRequest
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN must be 6 digits")]
        public string Pin { get; set; }
    }
}
