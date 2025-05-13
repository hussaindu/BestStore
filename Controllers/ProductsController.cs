using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStore_Application.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]

    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment Environment;
        private readonly int pageSize = 3;


        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {

            IQueryable<Product> query = _context.Products;

            //search function
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }
            //query = query.OrderByDescending(p => p.Id);

            //sort funtionality

            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreateAt" };
            string[] validOrderBy = { "desc", "asc" };
            query.OrderByDescending(p => p.Id);

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))

            {
                orderBy = "desc";
            }

            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }

            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }

            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }

            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }

            else if (column == "CreateAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreateAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreateAt);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }




            //pagination function

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;


            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFileName == null)
            {
                ModelState.AddModelError("ImageFileName", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFileName!.FileName);

            string imageFullPath = Environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFileName.CopyTo(stream);
            }
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Description = productDto.Description,
                Price = productDto.Price,
                Category = productDto.Category,
                ImageFileName = newFileName,
                CreateAt = DateTime.Now


            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateAt"] = product.CreateAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreateAt"] = product.CreateAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            string newFileName = product.ImageFileName;

            if (productDto.ImageFileName != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDto.ImageFileName.FileName);

                string imageFullPath = Path.Combine(Environment.WebRootPath, "products", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFileName.CopyTo(stream);
                }

                // Delete old image
                string oldImageFullPath = Path.Combine(Environment.WebRootPath, "products", product.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            // Update the product
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Category = productDto.Category;
            product.ImageFileName = newFileName;

            _context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }


        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }


            string imageFullPath = Environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Products");

        }

    }
}
