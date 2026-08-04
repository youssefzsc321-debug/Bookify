using CloudinaryDotNet;

namespace Bookify.Web.Services
{
    public interface IEmailBodyBulider
    {
        public string GetBody(string templete, Dictionary<string, string> placeholders);
    }
}
