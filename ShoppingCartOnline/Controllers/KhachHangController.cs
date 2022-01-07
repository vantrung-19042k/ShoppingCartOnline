using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;

namespace ShoppingCartOnline.Controllers
{
    public class KhachHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: KhachHang
        public ActionResult Index()
        {
            var lstKH = from kh in db.KhachHangs select kh;

            return View(lstKH);
        }
    }
}