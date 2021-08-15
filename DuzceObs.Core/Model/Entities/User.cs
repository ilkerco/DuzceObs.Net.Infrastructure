using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace DuzceObs.Core.Model.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Tc { get; set; }
        public string PhotoUrl { get; set; }
        public string UserType { get; set; }

    }
}
