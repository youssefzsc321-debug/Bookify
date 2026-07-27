using Microsoft.AspNetCore.Hosting;
using System.Text.Encodings.Web;

namespace Bookify.Web.Services
{
     
    public class EmailBodyBulider : IEmailBodyBulider
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailBodyBulider(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment=webHostEnvironment;
        }
        public string GetBody(string imageUrl, string header, string body, string url, string linkTitle)
        {
            var filPath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";
            StreamReader streamReader = new StreamReader(filPath);
            var Body = streamReader.ReadToEnd();
            streamReader.Close();


            Body = Body.Replace("[imageUrl]", imageUrl)
                .Replace("[header]",header)
                .Replace("[body]", body)
                .Replace("[url]", url)
                .Replace("[linkTitle]",linkTitle);

            return Body;
        }
    }
}
