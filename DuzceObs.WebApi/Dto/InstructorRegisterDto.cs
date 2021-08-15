using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class InstructorRegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "You must specify a password between 5 and 10 characters")]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Tc { get; set; }
        public string PhotoUrl { get; set; }

    }
}
