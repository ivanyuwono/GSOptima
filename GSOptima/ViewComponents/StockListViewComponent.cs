using GSOptima.Data;
using GSOptima.Models;
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
    public class StockListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _userID;

        public StockListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
           
        }

        public async Task<IViewComponentResult> InvokeAsync(int? page, string filter, string sort, string nowsort)
        {
            List<Stock> model;
            if (String.IsNullOrEmpty(filter))
            {
                model = await _context.Stock.ToListAsync();

            }
            else
            {
                model = await _context.Stock.Where(s => s.Name.Contains(filter) || s.StockID.Contains(filter)).ToListAsync();
             
            }

            Paging<Stock> pagingModel = new Paging<Stock>();
            pagingModel.data = (IQueryable<Stock>)model.AsQueryable();
            pagingModel.attribute.recordPerPage = 10;
            pagingModel.attribute.recordsTotal = model.Count();
            pagingModel.attribute.totalPage = Convert.ToInt32(Math.Ceiling((decimal)pagingModel.attribute.recordsTotal / pagingModel.attribute.recordPerPage));
            pagingModel.attribute.url = @"/StockWatchList/SearchStock";
            pagingModel.attribute.divName = "stockList";
            //pagingModel.attribute.sorting = sort;
            pagingModel.attribute.filter = filter;
            //pagingModel.OrderBy(sort);

            if (page == null)
                page = 1;


            pagingModel.DoPaging((int)page);
            return View(pagingModel);
        }

    }
}
