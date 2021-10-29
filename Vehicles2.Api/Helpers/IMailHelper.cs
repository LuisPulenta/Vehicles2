using Vehicles2.Common.Models;

namespace Vehicles2.Api.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string to, string subject, string body);
    }
}