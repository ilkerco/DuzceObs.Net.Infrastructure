using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Core.Model.Entities
{
    public class DersDegerlendirme:BaseEntity
    {
        public string Name { get; set; }
        public double Yuzde { get; set; }
        public virtual Ders Ders { get; set; }
        public int DersId { get; set; }
        public virtual List<Notlar> Notlar { get; set; }
    }
}
