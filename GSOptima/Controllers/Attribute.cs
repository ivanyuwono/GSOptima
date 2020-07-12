using GSOptima.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSOptima.Controllers
{
    public class SessionTimeoutAttributse : ActionFilterAttribute
    {
        //private readonly SignInManager<ApplicationUser> _signInManager;

        //public SessionTimeoutAttribute(SignInManager<ApplicationUser> signInManager)
        //{
        //    _signInManager = signInManager;
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //read cookie from IHttpContextAccessor  

            //string cookieValueFromContext =  filterContext.HttpContext.Request.Cookies["ivan2"];
            //if(cookieValueFromContext != null)
            //{
            //    //base.OnActionExecuting(filterContext);
            //}
            
            if (filterContext.HttpContext.Session.GetString("_UserId") == null)
            {
                //_signInManager.SignOutAsync();
                filterContext.Result = new RedirectResult("~/Account/Login");

                //RedirectResult("~/Account/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }

    

}
