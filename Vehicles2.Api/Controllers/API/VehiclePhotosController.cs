using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vehicles2.Api.Data;
using Vehicles2.Api.Data.Entities;
using Vehicles2.Api.Helpers;
using Vehicles2.API.Models.Request;

namespace Vehicles2.API.Controllers.API
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class VehiclePhotosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;

        public VehiclePhotosController(DataContext context, IImageHelper imageHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
        }

        [HttpPost]
        public async Task<ActionResult<VehiclePhoto>> PostVehiclePhoto(VehiclePhotoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Vehicle vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
            if (vehicle == null)
            {
                return BadRequest("El vehículo no existe.");
            }

            string imageId = _imageHelper.UploadImage(request.Image, "vehicles");
            VehiclePhoto vehiclePhoto = new()
            {
                ImageId = imageId,
                Vehicle = vehicle
            };

            _context.VehiclePhotos.Add(vehiclePhoto);
            await _context.SaveChangesAsync();
            return Ok(vehicle);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiclePhoto(int id)
        {
            VehiclePhoto vehiclePhoto = await _context.VehiclePhotos.FindAsync(id);
            if (vehiclePhoto == null)
            {
                return NotFound();
            }

            _context.VehiclePhotos.Remove(vehiclePhoto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}