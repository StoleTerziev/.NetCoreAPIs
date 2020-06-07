using AutoMapper;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Entities.Course, Models.CourseDTO>();
            CreateMap<Models.CourseForCreationDTO, Entities.Course>();
            CreateMap<Models.CourseForUpdateDTO, Entities.Course>();
        }
    }
}
