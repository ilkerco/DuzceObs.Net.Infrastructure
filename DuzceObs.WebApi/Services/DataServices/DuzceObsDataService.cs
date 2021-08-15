using AutoMapper;
using DuzceObs.Core.Model.Entities;
using DuzceObs.Core.Services.Interfaces;
using DuzceObs.Infrastructure.Data;
using DuzceObs.WebApi.Dto;
using DuzceObs.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Services.DataServices
{
    public class DuzceObsDataService : IDuzceObsDataService
    {
        private readonly IDersService _dersService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IDersDegerlendirmeService _dersDegerlendirmeService;
        private readonly IMapper _mapper;
        private readonly IStudentService _studentService;
        private readonly INotlarService _notlarService;
        public DuzceObsDataService(
            IDersService dersService,
            IUserService userService,
            UserManager<User> userManager,
            IStudentService studentService,
            IMapper mapper,
            INotlarService notlarService,
            IDersDegerlendirmeService dersDegerlendirmeService)
        {
            _dersService = dersService;
            _mapper = mapper;
            _notlarService = notlarService;
            _userManager = userManager;
            _studentService = studentService;
            _userService = userService;
            _dersDegerlendirmeService = dersDegerlendirmeService;
        }
        public async Task<bool> AddDers(AddDersDto dersDto)
        {
            try
            {
                var newDers = new Ders
                {
                    DersAdi = dersDto.DersAdi,
                    DersKodu = dersDto.DersKodu,
                    StartDay = dersDto.StartDay,
                    StartTime = dersDto.StartTime,
                    EndTime = dersDto.EndTime,
                    InstructorId = _userService.GetCurrentUser()
                };
                await _dersService.AddAsync(newDers);
                _dersService.Save();
                foreach(var kriter in dersDto.DersDegerlendirmes)
                {
                    await _dersDegerlendirmeService.AddAsync(new DersDegerlendirme
                    {
                        Ders = newDers,
                        Name = kriter.Name,
                        Yuzde = kriter.Yuzde
                    });
                }
                _dersDegerlendirmeService.Save();
                return true;
                

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddDersToOgrenci(int dersId, string ogrenciId)
        {
            try
            {
                var student = await _studentService.GetAll().Include(x=>x.Dersler).Where(x=>x.Id == ogrenciId).FirstOrDefaultAsync();
                var ders = await _dersService.GetAsync(dersId);
                //var ogrenci = (Student)await _userManager.Users.Where(x => x.Id == ogrenciId).FirstOrDefaultAsync();
                
                foreach(var existDers in student.Dersler)
                {
                    if(existDers.Id == dersId)
                    {
                        return false;
                    }
                }
                student.Dersler.Add(ders);
                await _studentService.UpdateAsync(student,student.Id);
                _studentService.Save();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddNotFromExcel(List<AddNotDto> notModels)
        {
            try
            {

                foreach(var notModel in notModels)
                {
                    var student = await _studentService.GetAll().Where(x => x.OgrNo == notModel.OgrenciNo).FirstOrDefaultAsync();
                    var ders = await _dersService.GetAll().Include(x=>x.DersDegerlendirmes).Where(x => x.DersKodu == notModel.DersKodu).FirstOrDefaultAsync();
                    foreach (KeyValuePair<string, double> entry in notModel.Notlar)
                    {
                        //ogr_NotDict2.Add(entry.Key, entry.Value);
                        foreach (var dersDegerlendirme in ders.DersDegerlendirmes)
                        {
                            if(entry.Key == dersDegerlendirme.Name)
                            {
                                var allNots = await _notlarService.GetAll()
                                    .Where(x=>x.DersDegerlendirmeId == dersDegerlendirme.Id && x.StudentId==student.Id).FirstOrDefaultAsync();
                                if (allNots == null)
                                {
                                    _notlarService.Add(new Notlar
                                    {
                                        DersDegerlendirmeId = dersDegerlendirme.Id,
                                        StudentId = student.Id,
                                        Not = entry.Value,
                                    });
                                }
                                else
                                {
                                    allNots.Not = entry.Value;
                                    await _notlarService.UpdateAsync(allNots, allNots.Id);
                                }

                                
                            }
                        }
                    }
                    
                    
                }
                await _notlarService.SaveChanges();


                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteDersFromOgrenci(int dersId, string ogrenciId)
        {
            try
            {
                var student = await _studentService.GetAll().Include(x => x.Dersler).Where(x => x.Id == ogrenciId).FirstOrDefaultAsync();
                var ders = await _dersService.GetAsync(dersId);
                var dersExist = student.Dersler.Select(x => x.Id == dersId).FirstOrDefault();
                if(!dersExist)
                {
                    return false;
                }
                student.Dersler.Remove(ders);
                await _studentService.UpdateAsync(student, student.Id);
                _studentService.Save();

                return true;

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<List<DersResponseModel>> GetAllDersByInstructor()
        {
            try
            {
                var dersList = await _dersService.GetAll().Include(x => x.Students).Include(x=>x.DersDegerlendirmes)
                    .Where(x => x.InstructorId == _userService.GetCurrentUser()).ToListAsync();
                var returnModel = _mapper.Map<List<DersResponseModel>>(dersList);
                return returnModel;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<DersResponseModel>> GetAllDersByStudent()
        {
            try
            {
                var dersList = await _studentService.GetAll().Include(x => x.Dersler).Where(x => x.Id == _userService.GetCurrentUser()).FirstOrDefaultAsync();
                var returnModel = _mapper.Map<List<DersResponseModel>>(dersList.Dersler);
                return returnModel;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<DersResponseWithGrades>> GetDersResponseWithGrades()
        {
            try
            {
                var dersResponseList = new List<DersResponseWithGrades>();

                var dersIds = await _dersService.GetAll().Where(x=>x.InstructorId == _userService.GetCurrentUser()).Select(x => x.Id).ToListAsync();

                foreach(var dersId in dersIds)
                {
                    var dersResponseItem = new DersResponseWithGrades();
                    var ders = await _dersService.GetAll().Where(x => x.Id == dersId).Include(x=>x.Students).Include(x=>x.DersDegerlendirmes).FirstOrDefaultAsync();
                    dersResponseItem.DersAdi = ders.DersAdi;
                    dersResponseItem.DersKodu = ders.DersKodu;
                    dersResponseItem.DersId = ders.Id;
                    foreach(var dersKriter in ders.DersDegerlendirmes)
                    {

                        dersResponseItem.DersKriters.Add(new DersKriter
                        {
                            DersDegerlendirmeId = dersKriter.Id,
                            DersDegerlendirmeName = dersKriter.Name,
                            Yuzde = dersKriter.Yuzde
                        });
                    }

                    foreach (var ogrenciInDers in ders.Students)
                    {
                        var notList = new List<NotDeneme>();
                        var notlar = await _notlarService.GetAll().Include(x => x.DersDegerlendirme).Where(x => x.DersDegerlendirme.DersId == dersId && x.StudentId == ogrenciInDers.Id).ToListAsync();
                        foreach (var dd in notlar)
                        {
                            
                            notList.Add(new NotDeneme
                            {
                                Not = dd.Not,
                                DersDegerlendirmeId = dd.DersDegerlendirmeId,
                                DersDegerlendirmeName = dd.DersDegerlendirme.Name,
                                Yuzde = dd.DersDegerlendirme.Yuzde
                            });
                            
                        }
                        dersResponseItem.Students.Add(new StudentDeneme
                        {
                            FirstName = ogrenciInDers.FirstName,
                            LastName = ogrenciInDers.LastName,
                            Id = ogrenciInDers.Id,
                            OgrNo = ogrenciInDers.OgrNo,
                            Sinif = ogrenciInDers.Sinif,
                            Notlar = notList

                        });
                    }
                    dersResponseList.Add(dersResponseItem);


                }
                /*
                var dersler = await _dersService.GetAll().Include(x=>x.Students)
                    .Include(x => x.DersDegerlendirmes).ThenInclude(y => y.Notlar).ToListAsync();

                
                foreach(var ders in dersler)
                {
                    for(int i = 0; i< ders.Students.Count; i++)
                    {
                        
                    }


                    var dersResponseItem = new DersResponseWithGrades();
                    dersResponseItem.DersAdi = ders.DersAdi;
                    dersResponseItem.DersKodu = ders.DersKodu;
                    dersResponseItem.DersId = ders.Id;
                    foreach(var nots in ders.DersDegerlendirmes)
                    {
                        foreach(var dd in nots.Notlar)
                        {
                            var notList = new List<NotDeneme>();
                            notList.Add(new NotDeneme
                            {
                                Not = dd.Not,
                                DersDegerlendirmeId = dd.DersDegerlendirmeId,
                                DersDegerlendirmeName = dd.DersDegerlendirme.Name
                            });
                            dersResponseItem.Students.Add(new StudentDeneme
                            {
                                FirstName = dd.Student.FirstName,
                                LastName = dd.Student.LastName,
                                Id = dd.Student.Id,
                                OgrNo = dd.Student.OgrNo,
                                Sinif = dd.Student.Sinif,
                                Notlar = notList
                                
                            });
                            
                            
                        }
                    }
                    dersResponseList.Add(dersResponseItem);
                }
               foreach(var dd in dersResponseList)
                {
                    for(int i = 0; i < dd.Students.Count; i++)
                    {
                        for(int j = i+1; j < dd.Students.Count; j++)
                        {
                            if(dd.Students[i].Id == dd.Students[j].Id)
                            {
                                foreach(var not in dd.Students[j].Notlar)
                                {

                                }
                                //dd.Students[i].Notlar.Add();
                            }
                        }
                    }
                }*/
                return dersResponseList;

            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<DersResponseWithGrades> GetSingleDersResponseWithGrades(int dersId)
        {
            try
            {
                    var dersResponseItem = new DersResponseWithGrades();
                    var ders = await _dersService.GetAll().Where(x => x.Id == dersId).Include(x => x.Students).Include(x => x.DersDegerlendirmes).FirstOrDefaultAsync();
                    dersResponseItem.DersAdi = ders.DersAdi;
                    dersResponseItem.DersKodu = ders.DersKodu;
                    dersResponseItem.DersId = ders.Id;
                    foreach (var dersKriter in ders.DersDegerlendirmes)
                    {

                        dersResponseItem.DersKriters.Add(new DersKriter
                        {
                            DersDegerlendirmeId = dersKriter.Id,
                            DersDegerlendirmeName = dersKriter.Name,
                            Yuzde = dersKriter.Yuzde
                        });
                    }

                    foreach (var ogrenciInDers in ders.Students)
                    {
                        var notList = new List<NotDeneme>();
                        var notlar = await _notlarService.GetAll().Include(x => x.DersDegerlendirme).Where(x => x.DersDegerlendirme.DersId == dersId && x.StudentId == ogrenciInDers.Id).ToListAsync();
                        foreach (var dd in notlar)
                        {

                            notList.Add(new NotDeneme
                            {
                                Not = dd.Not,
                                DersDegerlendirmeId = dd.DersDegerlendirmeId,
                                DersDegerlendirmeName = dd.DersDegerlendirme.Name
                            });

                        }
                        dersResponseItem.Students.Add(new StudentDeneme
                        {
                            FirstName = ogrenciInDers.FirstName,
                            LastName = ogrenciInDers.LastName,
                            Id = ogrenciInDers.Id,
                            OgrNo = ogrenciInDers.OgrNo,
                            Sinif = ogrenciInDers.Sinif,
                            Notlar = notList

                        });
                    }
                return dersResponseItem;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<StudentDersResponse>> GetStudentDersWithGrades()
        {
            try
            {
                var dersResponseList = new List<StudentDersResponse>();
                /*var notlar = await _notlarService.GetAll().Include(x => x.DersDegerlendirme)
                    .Include(x => x.DersDegerlendirme.Ders.DersDegerlendirmes).Include(x => x.DersDegerlendirme.Ders).Include(x => x.DersDegerlendirme.Ders.Instructor).Where(x => x.StudentId == _userService.GetCurrentUser()).ToListAsync();
                foreach(var not in notlar)
                {
                    var dersResponseItem = new StudentDersResponse();
                    dersResponseItem.DersAdi = not.DersDegerlendirme.Ders.DersAdi;
                    dersResponseItem.DersGunu = not.DersDegerlendirme.Ders.StartDay;
                    dersResponseItem.DersKodu = not.DersDegerlendirme.Ders.DersKodu;
                    foreach(var dersKriter in not.DersDegerlendirme.Ders.DersDegerlendirmes)
                    {
                        dersKriter.
                    }

                }*/
                var student = await _studentService.GetAll().Where(x=>x.Id == _userService.GetCurrentUser()).Include(x=>x.Dersler).FirstOrDefaultAsync();

                //var dersIds = await _dersService.GetAll().Where(x => x.InstructorId == _userService.GetCurrentUser()).Select(x => x.Id).ToListAsync();

                foreach (var dersId in student.Dersler)
                {
                    var dersResponseItem = new StudentDersResponse();
                    var ders = await _dersService.GetAll().Where(x => x.Id == dersId.Id).Include(x => x.Instructor).Include(x => x.DersDegerlendirmes).FirstOrDefaultAsync();
                    dersResponseItem.DersAdi = ders.DersAdi;
                    dersResponseItem.DersKodu = ders.DersKodu;
                    dersResponseItem.DersId = ders.Id;
                    dersResponseItem.DersGunu = ders.StartDay;
                    dersResponseItem.DersTarihi = ders.StartTime +" "+ ders.EndTime;
                    dersResponseItem.InstructorName = ders.Instructor.FirstName + " " + ders.Instructor.LastName;
                    foreach (var dersKriter in ders.DersDegerlendirmes)
                    {
                        double dersNot = double.NaN;
                        var not = await _notlarService.GetAll().Where(x => x.StudentId == _userService.GetCurrentUser() && x.DersDegerlendirmeId == dersKriter.Id).FirstOrDefaultAsync();
                        if(not != null)
                        {
                            dersNot = not.Not;
                        }
                        var dersKriterYeni = new DersKriters();
                        dersKriterYeni.DersDegerlendirmeId = dersKriter.Id;
                        dersKriterYeni.DersDegerlendirmeName = dersKriter.Name;
                        dersKriterYeni.Yuzde = dersKriter.Yuzde;
                        if (double.IsNaN(dersNot))
                        {
                            dersKriterYeni.Not = null;
                        }
                        else
                        {
                            dersKriterYeni.Not = dersNot;
                        }
                        dersResponseItem.DersKriters.Add(dersKriterYeni);
                        /*dersResponseItem.DersKriters.Add(new DersKriters
                        {
                            Not = dersNot != double.NaN ?dersNot : null,
                            DersDegerlendirmeId = dersKriter.Id,
                            DersDegerlendirmeName = dersKriter.Name,
                            Yuzde = dersKriter.Yuzde
                        });*/
                    }

                    dersResponseList.Add(dersResponseItem);


                }
                
                return dersResponseList;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
