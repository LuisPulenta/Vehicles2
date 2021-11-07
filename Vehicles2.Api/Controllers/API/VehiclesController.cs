﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vehicles2.Api.Data;
using Vehicles2.Api.Data.Entities;
using Vehicles2.Api.Helpers;
using Vehicles2.API.Models.Request;

namespace Vehicles2.Api.Controllers.Api
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IUserHelper _userHelper;

        public VehiclesController(DataContext context, IImageHelper imageHelper, IUserHelper userHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _userHelper = userHelper;
        }

        [HttpPost]
        public async Task<ActionResult<Vehicle>> PostVehicle(VehicleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VehicleType vehicleType = await _context.VehicleTypes.FindAsync(request.VehicleTypeId);
            if (vehicleType == null)
            {
                return BadRequest("El tipo de vehículo no existe.");
            }

            Brand brand = await _context.Brands.FindAsync(request.BrandId);
            if (brand == null)
            {
                return BadRequest("La marca no existe.");
            }

            User user = await _userHelper.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("El usuario no existe.");
            }

            Vehicle vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => x.Plaque.ToUpper() == request.Plaque.ToUpper());
            if (vehicle != null)
            {
                return BadRequest("Ya existe un vehículo con esa placa.");
            }

            string imageId = string.Empty;
            List<VehiclePhoto> vehiclePhotos = new();
            if (request.Image != null && request.Image.Length > 0)
            {
                imageId = _imageHelper.UploadImage(request.Image, "vehicles");
                vehiclePhotos.Add(new VehiclePhoto
                {
                    ImageId = imageId
                });
            }

            vehicle = new Vehicle
            {
                Brand = brand,
                Color = request.Color,
                Histories = new List<History>(),
                Line = request.Line,
                Model = request.Model,
                Plaque = request.Plaque,
                Remarks = request.Remarks,
                User = user,
                VehiclePhotos = vehiclePhotos,
                VehicleType = vehicleType,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return Ok(vehicle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle(int id, VehicleRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VehicleType vehicleType = await _context.VehicleTypes.FindAsync(request.VehicleTypeId);
            if (vehicleType == null)
            {
                return BadRequest("El tipo de vehículo no existe.");
            }

            Brand brand = await _context.Brands.FindAsync(request.BrandId);
            if (brand == null)
            {
                return BadRequest("La marca no existe.");
            }

            User user = await _userHelper.GetUserAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("El usuario no existe.");
            }

            Vehicle vehicle = await _context.Vehicles.FindAsync(request.Id);
            if (vehicle == null)
            {
                return BadRequest("El vehículo no existe.");
            }

            vehicle.Brand = brand;
            vehicle.Color = request.Color;
            vehicle.Line = request.Line;
            vehicle.Model = request.Model;
            vehicle.Plaque = request.Plaque;
            vehicle.Remarks = request.Remarks;
            vehicle.VehicleType = vehicleType;

            try
            {
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe esta marca.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            Vehicle vehicle = await _context.Vehicles
                .Include(x => x.VehiclePhotos)
                .Include(x => x.Histories)
                .ThenInclude(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}