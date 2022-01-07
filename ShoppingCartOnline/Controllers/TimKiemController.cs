using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;
using PagedList;

namespace ShoppingCartOnline.Controllers
{
    public class TimKiemController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: TimKiem
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa, int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //thực hiện chức năng phân trang
            //1.tạo biến số sản phẩm trên trang
            int PageSize = 2;

            //2.tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);

            //tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(n => n.TenSP.ToLower().Contains(sTuKhoa.ToLower()));

            ViewBag.TuKhoa = sTuKhoa;

            return View(lstSP.OrderBy(n => n.TenSP).ToPagedList(PageNumber, PageSize));
        }

        [HttpPost]
        public ActionResult LayTuKhoa(string sTuKhoa)
        {
            //gọi về hàm get tìm kiếm
            return RedirectToAction("KQTimKiem", new {@sTuKhoa=sTuKhoa});
        }

        public ActionResult KQTimKiemPartial(string sTuKhoa)
        {
            //tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(n => n.TenSP.ToLower().Contains(sTuKhoa.ToLower()));

            ViewBag.TuKhoa = sTuKhoa;

            return PartialView(lstSP.OrderBy(n => n.DonGia));
        }
    }
}