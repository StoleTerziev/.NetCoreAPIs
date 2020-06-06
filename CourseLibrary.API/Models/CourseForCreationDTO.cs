using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{

    [CourseTitleMustBeDifferentFromDescription(
        ErrorMessage = "The provided description should be different from the title.")]
    public class CourseForCreationDTO //: IValidatableObject
    {
        [Required(ErrorMessage ="You should fill out a title")]
        [MaxLength(100,ErrorMessage ="The title shouldn't have more than 100 characters")]
        public String Title { get; set; }

        [MaxLength(1500,ErrorMessage ="The description shouldn't have more than 1500 characters")]
        public String Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "The provided description should be different from the title.",
        //            new[] {"CourseForCreationDTO"});
        //    }
        //}
    }
}
