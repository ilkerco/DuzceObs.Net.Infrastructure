using DuzceObs.Core.Model.Entities;
using DuzceObs.Core.Services.Interfaces;
using DuzceObs.Infrastructure.Data;
using DuzceObs.Infrastructure.Data.Repositories;
namespace DuzceObs.Infrastructure.Services
{
    public class StudentService:Repository<Student>,IStudentService
    {
        public StudentService(DuzceObsDbContext context) : base(context)
        {

        }
    }
}
