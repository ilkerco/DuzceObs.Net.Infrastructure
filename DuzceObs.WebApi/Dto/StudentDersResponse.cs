using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class StudentDersResponse
    {
        public StudentDersResponse()
        {
            this.DersKriters = new List<DersKriters>();
        }
        public List<DersKriters> DersKriters { get; set; }
        public int DersId { get; set; }
        public string DersKodu { get; set; }
        public string DersAdi { get; set; }
        public string InstructorName { get; set; }
        public string DersTarihi { get; set; }
        public string DersGunu { get; set; }
    }
    
    public class DersKriters
    {
        public double? Not { get; set; }
        public double Yuzde { get; set; }
        public int DersDegerlendirmeId { get; set; }
        public string DersDegerlendirmeName { get; set; }
    }
}
