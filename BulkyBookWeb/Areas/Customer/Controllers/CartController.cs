using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Text;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Product")
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult Plus(int cartId)
        {
            ShoppingCart cartObj = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartObj.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartObj);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            ShoppingCart cartObj = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartObj != null)
            {
                cartObj.Count -= 1;
                if (cartObj.Count <= 0)
                {
                    //delete cart from list
                    _unitOfWork.ShoppingCart.Remove(cartObj);
                }
                else
                {
                    _unitOfWork.ShoppingCart.Update(cartObj);
                }
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            ShoppingCart cartObj = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartObj != null )
            {
                _unitOfWork.ShoppingCart.Remove(cartObj);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }


        #region Local Methods
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            } 
            else if (shoppingCart.Count > 50 &&  shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
        #endregion
    }
}
