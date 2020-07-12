using GSOptima.Data;
using GSOptima.Models;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace LoanOriginationSystem.ViewComponents
{
    public class BootstrapModalViewComponent: ViewComponent
    {

        public BootstrapModalViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(BootstrapModel m)
        {
            //Modal m = new GSOptima.ViewModels.Modal() { Body = body, Header = header };
            return View(m);
        }
    }
           
}
