using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Core.Model.Entities
{
    public class Notlar:BaseEntity
    {
        public double Not { get; set; }
        public int DersDegerlendirmeId { get; set; }
        public virtual DersDegerlendirme DersDegerlendirme { get; set; }
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}
