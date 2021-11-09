using System.ComponentModel.DataAnnotations;

namespace Vehicles2.API.Models.Request
{
    public class VehiclePhotoRequest
    {
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public byte[] Image { get; set; }
    }
}