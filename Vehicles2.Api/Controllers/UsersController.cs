using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles2.Api.Data;
using Vehicles2.Api.Data.Entities;
using Vehicles2.Api.Helpers;
using Vehicles2.Api.Models;
using Vehicles2.Common.Enums;

namespace Vehicles.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public UsersController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IConverterHelper converterHelper, IImageHelper imageHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(x => x.DocumentType)
                .Include(x => x.Vehicles)
                .Where(x => x.UserType == UserType.User)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            UserViewModel model = new UserViewModel
            {
                DocumentTypes = _combosHelper.GetComboDocumentTypes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageId = string.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _converterHelper.ToUserAsync(model, imageId, true);
                user.UserType = UserType.User;
                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());

                return RedirectToAction(nameof(Index));
            }

            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel model = _converterHelper.ToUserViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageId = model.ImageId;
                if (model.ImageFile != null)
                {
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _converterHelper.ToUserAsync(model, imageId, false);
                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }

            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userHelper.DeleteUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _context.Users
                .Include(x => x.DocumentType)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Brand)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehicleType)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehiclePhotos)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Histories)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> AddVehicle(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _context.Users
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            VehicleViewModel model = new VehicleViewModel
            {
                Brands = _combosHelper.GetComboBrands(),
                UserId = user.Id,
                VehicleTypes = _combosHelper.GetComboVehicleTypes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(VehicleViewModel vehicleViewModel)
        {
            User user = await _context.Users
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == vehicleViewModel.UserId);
            if (user == null)
            {
                return NotFound();
            }

            string imageId = string.Empty;
            if (vehicleViewModel.ImageFile != null)
            {
                imageId = await _imageHelper.UploadImageAsync(vehicleViewModel.ImageFile, "vehicles");
            }

            Vehicle vehicle = await _converterHelper.ToVehicleAsync(vehicleViewModel, true);
            if (vehicle.VehiclePhotos == null)
            {
                vehicle.VehiclePhotos = new List<VehiclePhoto>();
            }

            vehicle.VehiclePhotos.Add(new VehiclePhoto
            {
                ImageId = imageId
            });

            try
            {
                user.Vehicles.Add(vehicle);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = user.Id });
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esa placa.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }

            vehicleViewModel.Brands = _combosHelper.GetComboBrands();
            vehicleViewModel.VehicleTypes = _combosHelper.GetComboVehicleTypes();
            return View(vehicleViewModel);
        }

        public async Task<IActionResult> EditVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .Include(x => x.User)
                .Include(x => x.Brand)
                .Include(x => x.VehicleType)
                .Include(x => x.VehiclePhotos)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            VehicleViewModel model = _converterHelper.ToVehicleViewModel(vehicle);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(int id, VehicleViewModel vehicleViewModel)
        {
            if (id != vehicleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Vehicle vehicle = await _converterHelper.ToVehicleAsync(vehicleViewModel, false);
                    _context.Vehicles.Update(vehicle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = vehicleViewModel.UserId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esta placa.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            vehicleViewModel.Brands = _combosHelper.GetComboBrands();
            vehicleViewModel.VehicleTypes = _combosHelper.GetComboVehicleTypes();
            return View(vehicleViewModel);
        }

        public async Task<IActionResult> DeleteVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .Include(x => x.User)
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
            return RedirectToAction(nameof(Details), new { id = vehicle.User.Id });
        }

        public async Task<IActionResult> DeleteImageVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VehiclePhoto vehiclePhoto = await _context.VehiclePhotos
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehiclePhoto == null)
            {
                return NotFound();
            }

            _context.VehiclePhotos.Remove(vehiclePhoto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditVehicle), new { id = vehiclePhoto.Vehicle.Id });
        }

        public async Task<IActionResult> AddVehicleImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            VehiclePhotoViewModel model = new()
            {
                VehicleId = vehicle.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicleImage(VehiclePhotoViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "vehicles");
                Vehicle vehicle = await _context.Vehicles
                    .Include(x => x.VehiclePhotos)
                    .FirstOrDefaultAsync(x => x.Id == model.VehicleId);
                if (vehicle.VehiclePhotos == null)
                {
                    vehicle.VehiclePhotos = new List<VehiclePhoto>();
                }

                vehicle.VehiclePhotos.Add(new VehiclePhoto
                {
                    ImageId = imageId
                });

                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditVehicle), new { id = vehicle.Id });
            }
            return View(model);
        }
    }
}