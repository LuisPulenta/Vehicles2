using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vehicles2.Api.Data.Entities
{
    public class VehiclePhoto
    {
        public int Id { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public Vehicle Vehicle { get; set; }

        [Display(Name = "Foto")]
        public string ImageId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImageId == string.Empty
           ? $"http://keypress.serveftp.net:88/Vehicles2Api/images/noimage.png"
            : $"http://keypress.serveftp.net:88/Vehicles2Api{ImageId.Substring(1)}";
    }
}