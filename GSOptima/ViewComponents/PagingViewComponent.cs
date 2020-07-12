using GSOptima.Data;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace GSOptima.ViewComponents
{
    public class Paging: ViewComponent
    {

        private readonly ApplicationDbContext _context;

        public Paging(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(PagingAttribute pagingAttribute)
        {
            return View(pagingAttribute);
        }
    }
           
}
