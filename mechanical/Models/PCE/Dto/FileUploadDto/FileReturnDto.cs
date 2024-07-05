using System;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Enum.File;

namespace mechanical.Models.PCE.Dto.FileUploadDto
{

    public class FileReturnDto
    {
        [Display(Name = "File Id")]
        public Guid Id { get; set; } 

        [Display(Name = "File Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Document Category")]
        public Category Category { get; set; }

        public string ContentType { get; set; } = string.Empty;

        // public Guid? PCEEId { get; set; }
    }

}
