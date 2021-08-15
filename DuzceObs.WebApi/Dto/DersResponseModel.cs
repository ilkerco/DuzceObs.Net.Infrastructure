using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class DersResponseModel
    {
        public int Id { get; set; }
        public string DersKodu { get; set; }
        public string DersAdi { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDay { get; set; }
        public string InstructorId { get; set; }
        public int StudentsCount { get; set; }
        public List<DersDegerlendirmeDto> DersDegerlendirmes { get; set; }
        public List<StudentResponse> Students { get; set; }
    }
}
