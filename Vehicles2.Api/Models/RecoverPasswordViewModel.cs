﻿using System.ComponentModel.DataAnnotations;

namespace Vehicles2.Api.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes introducir un email válido.")]
        public string Email { get; set; }
    }
}