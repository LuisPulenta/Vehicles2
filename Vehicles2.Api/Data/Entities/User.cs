using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vehicles2.Common.Enums;

namespace Vehicles2.Api.Data.Entities
{
    public class User : IdentityUser
    {
        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; }

        [Display(Name = "Tipo de documento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DocumentType DocumentType { get; set; }

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Document { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string Address { get; set; }

        [Display(Name = "Foto")]
        public string ImageId { get; set; }


        //TODO: Corregir ruta
        [Display(Name = "Foto")]
        public string ImageFullPath => string.IsNullOrEmpty(ImageId)
                     ? "https://localhost:44351/images/nouser.png"
                    : $"https://localhost:44354{ImageId.Substring(1)}";
        //: $"http://keypress.serveftp.net:88/Vehicles2Api/Images/users/{ImageId}";
        
        [Display(Name = "Tipo de usuario")]
        public UserType UserType { get; set; }

        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Vehicle> Vehicles { get; set; }

        [Display(Name = "N° Vehículos")]
        public int VehiclesCount => Vehicles == null ? 0 : Vehicles.Count;

    }
}