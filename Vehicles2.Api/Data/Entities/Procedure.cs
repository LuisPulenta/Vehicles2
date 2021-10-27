using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vehicles2.Api.Data.Entities
{
    [Index(nameof(Description), IsUnique = true)]
    public class Procedure
    {
        public int Id { get; set; }

        [Display(Name = "Procedimiento")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Description { get; set; }

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Price { get; set; }

        [JsonIgnore]
        public ICollection<Detail> Details { get; set; }
    }
}