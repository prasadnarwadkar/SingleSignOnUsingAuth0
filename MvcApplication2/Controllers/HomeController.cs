using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return new ChallengeResult("Saml2", Url.Action("Index", "Home"));
            }
        }

        [AllowAnonymous]
        public ActionResult Default2()
        {
            return View();
        }


    }
}