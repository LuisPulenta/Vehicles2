using System.Threading.Tasks;
using Vehicles2.Api.Data.Entities;
using Vehicles2.Api.Models;

namespace Vehicles2.Api.Helpers
{
    public interface IConverterHelper
    {
        Task<User> ToUserAsync(UserViewModel model, string imageId, bool isNew);

        UserViewModel ToUserViewModel(User user);
    }
}