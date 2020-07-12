using GSOptima.Data;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace GSOptima.ViewComponents
{
    public class ModalViewComponent: ViewComponent
    {

        public ModalViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string header, string body)
        {
            Modal m = new GSOptima.ViewModels.Modal() { Body = body, Header = header };
            return View(m);
        }
    }
           
}
