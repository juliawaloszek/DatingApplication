using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.DatingApp.Controllers
{
    public class FallBack: Controller
    //klasa pozwalająca na przekierowanie rządań i przekazanie do 
    //index.html który odpowiada za aplikacje angularową
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}