using DuzceObs.Core.Model.Entities;
using DuzceObs.Core.Services.Interfaces;
using DuzceObs.Infrastructure.Data;
using DuzceObs.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuzceObs.Infrastructure.Services
{
    public class DersDegerlendirmeService:Repository<DersDegerlendirme>,IDersDegerlendirmeService
    {
        public DersDegerlendirmeService(DuzceObsDbContext context) : base(context) { }
    }
}
