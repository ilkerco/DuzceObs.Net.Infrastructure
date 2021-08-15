using DuzceObs.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Services.Interfaces
{
    public interface IDuzceObsDataService
    {
        Task<bool> AddDers(AddDersDto dersDto);
        Task<List<DersResponseModel>> GetAllDersByInstructor();
        Task<List<DersResponseModel>> GetAllDersByStudent();
        Task<bool> AddDersToOgrenci(int dersId, string ogrenciId);
        Task<bool> DeleteDersFromOgrenci(int dersId, string ogrenciId);
        Task<List<DersResponseWithGrades>> GetDersResponseWithGrades();
        Task<DersResponseWithGrades> GetSingleDersResponseWithGrades(int dersId);
        Task<List<StudentDersResponse>> GetStudentDersWithGrades();
        Task<bool> AddNotFromExcel(List<AddNotDto> notModels);
    }
}
