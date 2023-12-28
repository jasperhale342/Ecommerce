using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Utility;



namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;   
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0) {
                return View(productVM);

            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id, includeProperties:"ProductImages");
                return View(productVM);

            }
           

        }


        [HttpPost]
        public IActionResult Upsert(ProductVM productViewModel, List<IFormFile> files)
        {
           
            if (ModelState.IsValid)
            {
                if (productViewModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productViewModel.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }

                _unitOfWork.Save();
                string wwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files) {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productViewModel.Product.Id;
                        string finalPath = Path.Combine(wwRootPath, productPath);
                        if (! Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);

                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);

                        }
                        ProductImage productImage = new ProductImage
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productViewModel.Product.Id,
                        };

                        if(productViewModel.Product.ProductImages == null)
                        {
                            productViewModel.Product.ProductImages = new List<ProductImage>();  

                        }

                        productViewModel.Product.ProductImages.Add(productImage);
                           

                        


                    }
                    _unitOfWork.Product.Update(productViewModel.Product);
                    _unitOfWork.Save();
                    

                   
                }

                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productViewModel);  
               
            }

           
        }
       
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");



        }


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u=>u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (! string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, 
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                    _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                    _unitOfWork.Save();
                    TempData["success"] = "Deleted successfully";
                }

            }
            return RedirectToAction(nameof(Upsert), new {id=productId});
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted  = _unitOfWork.Product.Get(u=> u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while Deleting"});
            }
           

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);

            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();


            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
