using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SMS.Web.Controllers
{
    public enum AlertType { success, danger, warning, info }

    public class BaseController : Controller
    {
        public void Alert(string message, AlertType type = AlertType.info)
        {
            TempData["Alert.Message"] = message;
            TempData["Alert.Type"] = type.ToString();
        }

    }
       
}
