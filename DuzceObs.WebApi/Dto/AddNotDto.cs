using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Dto
{
    public class AddNotDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty("Ogrenci No")]
        public string OgrenciNo { get; set; }

        [JsonProperty("Ders Kodu")]
        public string DersKodu { get; set; }

        [JsonProperty("Ders Adi")]
        public string DersAdi { get; set; }
        public Dictionary<string,double> Notlar { get; set; }
    }
}
