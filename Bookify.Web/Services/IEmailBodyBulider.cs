using CloudinaryDotNet;

namespace Bookify.Web.Services
{
    public interface IEmailBodyBulider
    {
        public string GetBody(string imageUrl,string header,string body,string url,string linkTitle);
    }
}
