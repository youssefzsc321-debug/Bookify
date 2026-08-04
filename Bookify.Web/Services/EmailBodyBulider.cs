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
        public string GetBody(string templete,Dictionary<string,string>placeholders)
        {
            var filPath = $"{_webHostEnvironment.WebRootPath}/templates/{templete}.html";
            StreamReader streamReader = new StreamReader(filPath);
            var Body = streamReader.ReadToEnd();
            streamReader.Close();

            foreach(var placeholder in placeholders)
            {
               Body=Body.Replace(placeholder.Key, placeholder.Value); 
            }

            return Body;
        }
    }
}
