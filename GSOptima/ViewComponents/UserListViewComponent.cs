using GSOptima.Data;
using GSOptima.Models;
using GSOptima.Models.AccountViewModels;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSOptima.ViewComponents
{
    public class UserListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _userID;

        public UserListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

           
        }

        // GET: StockWatchLists
        public async Task<IViewComponentResult> InvokeAsync(int? page, string filter, string sort, string nowsort)
        {
           
            var _users = await _userManager.Users.ToListAsync();
            List<RegisterViewModel> reg = new List<RegisterViewModel>();
            if (!String.IsNullOrEmpty(filter))
            {
                _users = _users.Where(p => p.Name.Contains(filter) || p.Email.Contains(filter)).ToList();
            }
            else
                filter = "";

            foreach (var user in _users)
            {

                List<string> str_roles = new List<string>();
                var roles = await _userManager.GetRolesAsync(user);

                reg.Add(new RegisterViewModel()
                {
                    Address = user.Address,
                    Name = user.Name,
                    Email = user.Email,
                    StartDate = user.StartDate,
                    EndDate = user.EndDate,
                    Roles = roles.Count > 0 ? roles.First() : "",
                    Id = user.Id,
                    EmailConfirmed =user.EmailConfirmed,
                    MembershipType = user.MembershipType });

            }
            var model = reg;

            //var watchList = _context.StockWatchList.Where(s => s.ApplicationUserId == currentUser.Id).Include(s => s.Stock);


            if (string.IsNullOrEmpty(sort))
                sort = "Email";

            var current_sort_field = sort.Replace("_DESC", "");

            if (!string.IsNullOrEmpty(nowsort))
            {


                if (nowsort == current_sort_field)
                {
                    if (sort.Contains("_DESC"))
                        sort = sort.Replace("_DESC", "");
                    else
                        sort = sort + "_DESC";
                }
                else
                    sort = nowsort;
            }
            

            Paging<RegisterViewModel> pagingModel = new Paging<RegisterViewModel>();
            pagingModel.data = (IQueryable<RegisterViewModel>)model.AsQueryable();
            pagingModel.attribute.recordPerPage = 25;
            pagingModel.attribute.recordsTotal = model.Count();
            pagingModel.attribute.totalPage = Convert.ToInt32(Math.Ceiling((decimal)pagingModel.attribute.recordsTotal / pagingModel.attribute.recordPerPage));
            pagingModel.attribute.url = @"/Account/Index";
            pagingModel.attribute.divName = "userList";
            pagingModel.attribute.sorting = sort;
            pagingModel.attribute.filter = filter;
            pagingModel.OrderBy(sort);

            if (page == null)
                page = 1;

            
            pagingModel.DoPaging((int)page);
            return View(pagingModel);


            //return View(model);
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IViewComponentResult> DeleteAsync([FromBody] string stockId)
        //{
        //    var watchList = await _context.StockWatchList.SingleOrDefaultAsync(s => s.ApplicationUserId == _userID.Id && s.StockID == stockId);
        //    _context.StockWatchList.Remove(watchList);
        //    await _context.SaveChangesAsync();
        //    return View();
        //}
    }
}
