﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return View(objProductList);
		}

		// GET
		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};
			if (id == null || id == 0)
			{
				// create
				return View(productVM);
			}
			else
			{
				// update
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}
		}

		// POST
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, List<IFormFile?> files)
		{
			if (ModelState.IsValid)
			{
				if (productVM.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
				}

				_unitOfWork.Save();

				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (files != null)
				{
					foreach (var file in files)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string productPath = @"images\product\product-" + productVM.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);

						if (!Directory.Exists(finalPath)) 
							Directory.CreateDirectory(finalPath);

						// Copy image file to the product image directory
						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						}

						ProductImage productImage = new()
						{
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId = productVM.Product.Id
						};

						if (productVM.Product.ProductImages == null)
							productVM.Product.ProductImages = new List<ProductImage>();

						productVM.Product.ProductImages.Add(productImage);
					}

					_unitOfWork.Product.Update(productVM.Product);
					_unitOfWork.Save();
				}

				TempData["success"] = productVM.Product.Id == 0 ? "Product created successfully" : "Product updated successfully";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
			}
			return View(productVM);
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProductList });
		}

		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			//var oldImagePath =
			//	Path.Combine(_webHostEnvironment.WebRootPath, 
			//				productToBeDeleted.ImageUrl.TrimStart('\\'));

			//if (System.IO.File.Exists(oldImagePath))
			//{
			//	System.IO.File.Delete(oldImagePath);
			//}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}

		#endregion
	}
}
