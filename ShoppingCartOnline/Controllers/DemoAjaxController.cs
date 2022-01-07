using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;

namespace ShoppingCartOnline.Controllers
{
    public class DemoAjaxController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: DemoAjax
        public ActionResult DemoAjax()
        {
            return View();
        }

        //Xử lý Ajax Action Link
        public ActionResult LoadAjaxActionLink1()
        {
            return Content("Hello Ajax 1"); 
        }

        public ActionResult LoadAjaxActionLink2()
        {
            return Content("Hello Ajax 2");
        }

        //Xử lý Ajax BeginForm
        public ActionResult LoadAjaxBeginForm(FormCollection f)
        {
            string kq = f["txt1"].ToString();
            return Content(kq);
        }

        //Xử lý Ajax Jquery
        public ActionResult LoadAjaxJquery(int a, int b)
        {
            System.Threading.Thread.Sleep(2000); //ngủ 2s
            return Content((a + b).ToString());
        }

        //Sử dụng load Ajax trả về kết quả là một partial view
        public ActionResult SanPhamPartialAjax()
        {
            var lstSanPham = db.SanPhams.Where(p => p.MaSP==1);

            return PartialView(lstSanPham);
        }
    }
}