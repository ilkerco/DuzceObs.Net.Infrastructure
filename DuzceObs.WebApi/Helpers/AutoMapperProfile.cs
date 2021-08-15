using AutoMapper;
using DuzceObs.Core.Model.Entities;
using DuzceObs.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;

            CreateMap<InstructorRegisterDto, User>(MemberList.None);
            CreateMap<User, InstructorDto>();
            CreateMap<User, StudentResponse>();
            CreateMap<InstructorRegisterDto, Student>(MemberList.None);
            CreateMap<Student, StudentDto>();
            CreateMap<StudentDto, Student>();
            CreateMap<InstructorRegisterDto, Instructor>(MemberList.None);
            CreateMap<Instructor, InstructorDto>();
            CreateMap<Ders, DersResponseModel>()
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Students.Count));
            //CreateMap<User, UserResponse>();
        }
    }
}
