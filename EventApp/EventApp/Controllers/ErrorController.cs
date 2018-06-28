using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventApp.Controllers
{
    public sealed class ErrorController : Controller
    {
        // 401-403-500 hatalarını da ekleyebilirsin sonra
        public ActionResult Http404()
        {
            ActionResult result;

            object model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                result = View(model);
            else
                result = PartialView("_NotFound", model);

            return result;
        }
    }
}