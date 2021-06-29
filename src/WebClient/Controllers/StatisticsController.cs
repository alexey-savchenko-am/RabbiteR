using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebClient.Data;

namespace WebClient
{
    public class StatisticsController
        : Controller
    {
        private readonly ShopContext _shopContext;

        public StatisticsController(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        public ActionResult Info()
        {

            return View(_shopContext.Orders.ToList());
        }
    }
}
