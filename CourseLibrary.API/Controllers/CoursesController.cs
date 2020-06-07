using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId,
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

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);

                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDTO>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                   new { authorId,courseId = courseToReturn.Id },courseToReturn );
            }

            _mapper.Map(course, courseForAuthorFromRepo);

            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            _courseLibraryRepository.Save();
            return NoContent();
        }


        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor (Guid authorId,
            Guid courseId,
            JsonPatchDocument<CourseForUpdateDTO> patchDocument)
        {
            if(!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if(courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var courseToPatch = _mapper.Map<CourseForUpdateDTO>(courseForAuthorFromRepo);

            //add validation
            patchDocument.ApplyTo(courseToPatch);

            _mapper.Map(courseToPatch, courseForAuthorFromRepo);

            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            _courseLibraryRepository.Save();

            return NoContent();
        }
    }
}
