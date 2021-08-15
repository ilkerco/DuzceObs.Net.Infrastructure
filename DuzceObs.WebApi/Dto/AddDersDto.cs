using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class AddDersDto
    {
        public string DersKodu { get; set; }
        public string DersAdi { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDay { get; set; }
        public string InstructorId { get; set; }
        public List<DersDegerlendirmeDto> DersDegerlendirmes {get;set;}
    }
    public class DersDegerlendirmeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Yuzde { get; set; }
    }
}
