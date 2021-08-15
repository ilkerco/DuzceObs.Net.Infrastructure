using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Core.Model.Entities
{
    public class Ders:BaseEntity
    {
        public Ders()
        {
            this.Students = new HashSet<Student>();
            this.DersDegerlendirmes = new HashSet<DersDegerlendirme>();
        }
        public string DersKodu { get; set; }
        public string DersAdi { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDay { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public string InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual ICollection<DersDegerlendirme> DersDegerlendirmes { get; set; }
    }
}
