using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly int pageSize = 4;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort )
        {

            IQueryable<Product> query = _context.Products;

            //search fuction

            if (search != null && search.Length > 0)
            { 
                query=query.Where(x => x.Name.Contains(search));
            }


            //filter function

            if(brand!=null && brand.Length > 0)
            {
                query=query.Where(x => x.Brand.Contains(brand));    
            }

            if (category != null && category.Length > 0) 
            {
                query=query.Where(p=>p.Category.Contains(category));
            }



            // sort function

            if(sort == "price_asc")
            {
                query=query.OrderBy(p=>p.Price);
            }
            else if(sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                query=query.OrderByDescending(p => p.Price);
            }



            query = query.OrderByDescending(p => p.Id);

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products=query.ToList();

            ViewBag.Products = products;
            ViewBag.TotalPages = totalPages;
            ViewBag.pageIndex = pageIndex;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort
            };
            return View(storeSearchModel);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Store");
            }
            return View(product);
        }
    }
}
