using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class StudentResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Tc { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OgrNo { get; set; }
        public int Sinif { get; set; }
        public string PhotoUrl { get; set; }
    }
}
