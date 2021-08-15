using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class DersResponseWithGrades
    {
        public DersResponseWithGrades()
        {
            this.Students = new List<StudentDeneme>();
            this.DersKriters = new List<DersKriter>();
        }
        public List<DersKriter> DersKriters { get; set; }
        public int DersId { get; set; }
        public string DersKodu { get; set; }
        public string DersAdi { get; set; }
        public List<StudentDeneme> Students { get; set; }
    }
    public class StudentDeneme
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Sinif { get; set; }
        public string OgrNo { get; set; }
        public List<NotDeneme> Notlar { get; set; }
    }
    public class NotDeneme
    {
        public double Not { get; set; }
        public int DersDegerlendirmeId { get; set; }
        public string DersDegerlendirmeName { get; set; }
        public double Yuzde { get; set; }
    }
    public class DersKriter
    {
        public double Yuzde { get; set; }
        public int DersDegerlendirmeId { get; set; }
        public string DersDegerlendirmeName { get; set; }
    }
}
