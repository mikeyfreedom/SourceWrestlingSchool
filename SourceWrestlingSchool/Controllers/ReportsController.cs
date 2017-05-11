using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Revenue()
        {
            //Get Event Revenue
            //Get Membership Revenue
            //Get Fines and Private Revenue
            //Create ViewModel
            //Add Data to Sending Model
            return View();
        }
    }
}