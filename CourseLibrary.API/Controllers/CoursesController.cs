using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {

        private ICourseLibraryRepository _courseLibraryRepository;
        private IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepositry,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepositry ??
                throw new ArgumentNullException(nameof(courseLibraryRepositry));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDTO>> GetCoursesForAuthor(Guid authorId)
        {
            if(!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var coursesForAuthorFromRepo = _courseLibraryRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDTO>>(coursesForAuthorFromRepo));
        }

        [HttpGet("{courseId}",Name ="GetCourseForAuthor")]
        public ActionResult<IEnumerable<CourseDTO>> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);
            if(courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseDTO>(courseForAuthorFromRepo));
        }


        [HttpPost]
        public ActionResult<CourseDTO> CreateCourseForAuthor(
            Guid authorId, CourseForCreationDTO course)
        {
            if(!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Entities.Course>(course);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<Models.CourseDTO>(courseEntity);
            return CreatedAtRoute("GetCourseForAuthor",
                new { authorId = authorId,courseId = courseToReturn.Id},courseToReturn);

        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId,
            CourseForUpdateDTO course)
        {
            if(!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId); 

            if(courseForAuthorFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;
            }

            _mapper.Map(course, courseForAuthorFromRepo);

            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            _courseLibraryRepository.Save();
            return NoContent();
        }
    }
}
