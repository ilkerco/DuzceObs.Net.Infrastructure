using DuzceObs.Core.Model.Entities;
using DuzceObs.Core.Services.Interfaces;
using DuzceObs.Infrastructure.Data;
using DuzceObs.Infrastructure.Data.Repositories;

namespace DuzceObs.Infrastructure.Services
{
    public class NotlarService:Repository<Notlar>,INotlarService
    {
        public NotlarService(DuzceObsDbContext context) : base(context) { }
    }
}
