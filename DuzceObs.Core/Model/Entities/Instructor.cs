using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Core.Model.Entities
{
    public class Instructor:User
    {
        public Instructor()
        {
            this.Dersler = new HashSet<Ders>();
        }
        public virtual ICollection<Ders> Dersler { get; set; }
    }
}
