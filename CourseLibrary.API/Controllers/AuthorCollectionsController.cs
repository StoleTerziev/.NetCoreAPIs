using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{

    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private ICourseLibraryRepository _courseLibraryRepository;
        private IMapper _mapper;



        public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepositry,
          IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepositry ??
                throw new ArgumentNullException(nameof(courseLibraryRepositry));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet("({ids})",Name ="GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
            [FromRoute] 
            [ModelBinder(BinderType =typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _courseLibraryRepository.GetAuthors(ids);

            if(ids.Count()!= authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDTO>>(authorEntities);
            return Ok();
        }


        [HttpPost]
        public ActionResult<IEnumerable<AuthorDTO>> CreateAuthorCollecion(
            IEnumerable<AuthorForCreationDTO>authorCollecion)
        {

            var authorEntities = _mapper.Map<IEnumerable<Entities.Author>>(authorCollecion);
            foreach(var author in authorEntities)
            {
                _courseLibraryRepository.AddAuthor(author);
            }

            _courseLibraryRepository.Save();

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDTO>>(authorEntities);
            var idsAsString = string.Join(",",authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection",
                new { ids = idsAsString }, authorCollectionToReturn);

        }

    }
}
