using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Core.Model.Entities
{
    public class Student:User
    {
        public Student()
        {
            this.Dersler = new HashSet<Ders>();
        }
        public string OgrNo { get; set; }
        public int Sinif { get; set; }
        public virtual ICollection<Ders> Dersler { get; set; }
        public virtual List<Notlar> Notlars { get; set; }
    }
}
