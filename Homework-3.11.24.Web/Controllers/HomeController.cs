using Homework_3._11._24.Data;
using Homework_3._11._24.Web.Controllers;
using Homework_3._11._24.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Homework_3._11._24.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            using FileStream fs = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName), FileMode.Create);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository();

            return View(new ImageViewModel
            {
                Image = repo.GetImage(repo.Add(new Image { Password = password, FileName = fileName }))
            });
        }

        public IActionResult ViewImage(Image image, string enteredPassword)
        {
            var repo = new ImageRepository();
            var ImageIds = HttpContext.Session.Get<List<int>>("ids") != null ? HttpContext.Session.Get<List<int>>("ids") : new List<int>();

            if (enteredPassword != null && image.Password == enteredPassword && !ImageIds.Contains(image.Id))
            {
                ImageIds.Add(image.Id);
                HttpContext.Session.Set("ids", ImageIds);
            }

            if (ImageIds.Contains(image.Id))
            {
                repo.UpdateImageViews(image.Id);
            }

            return View(new ImageViewModel
            {
                Image = repo.GetImage(image.Id),
                PasswordPreviouslyEntered = ImageIds.Contains(image.Id),
                Message = ImageIds.Contains(image.Id) || image.Password == null ? "" : "Invalid Password"
            });
        }
    }
}


public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
    }
}



