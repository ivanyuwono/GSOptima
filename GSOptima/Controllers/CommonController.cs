using GSOptima.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSOptima.Controllers
{
    public class CommonController : Controller
    {
        [HttpPost]
        public IActionResult ShowModal(string header, string body)
        {


            //if (Request?.Headers != null &&
            //    Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return ViewComponent("StockList", new { search = search });

            //}
            //return ViewComponent("Modal", new { header = header, body = body });
            return PartialView("_MessageModal", new BootstrapModel {Header=header,  Message = body } );

        }
    }
}
