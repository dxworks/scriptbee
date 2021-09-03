using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ScriptBeeWebApp.Extensions
{
    public static class FormFileExtension
    {
        public static Task<string> ReadFormFileContent(this IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}