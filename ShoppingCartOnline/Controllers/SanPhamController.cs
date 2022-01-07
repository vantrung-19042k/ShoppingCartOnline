using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;
using System.Net;
using PagedList;

namespace ShoppingCartOnline.Controllers
{
    public class SanPhamController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        //Tạo 2 partial view để hiển thị sản phẩm theo 2 style khác nhau
        [ChildActionOnly]
        public ActionResult SanPhamStyle1Partial()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult SanPhamStyle2Partial()
        {
            return PartialView();
        }

        //Xây dựng trang xem chi tiết
        public ActionResult XemChiTiet(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //nếu không thì truy xuất csdl lấy ra sản phẩm tương ứng
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP  == id);
            if(sp == null)
            {
                return new HttpNotFoundResult();
            }

            return View(sp);
        }

        //Xây dựng 1 action load sản phẩm theo mã loại sản phẩm và mã nhà sản xuất
        public ActionResult SanPham(int? MaloaiSP, int? MaNSX, int? page)
        {
            ////Kiểm tra user đã đăng nhập hay chưa (test)
            //if(Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            /* Load sản phẩm dựa theo hai tiêu chí là mã loại sản phẩm và mã nhà sản xuất
              2 trường trong bảng sản phẩm */
            if (MaloaiSP == null || MaNSX == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaloaiSP && n.MaNSX == MaNSX);

            if (lstSP.Count() == 0)
            {
                return new HttpNotFoundResult();
            }

            if(Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //thực hiện chức năng phân trang
            //1.tạo biến số sản phẩm trên trang
            int PageSize = 3;

            //2.tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaloaiSP;
            ViewBag.MaNSX = MaNSX;

            return View(lstSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }  
        
        public ActionResult SanPhamTheoLoai(int? MaLoaiSP, int?page)
        {
            if (MaLoaiSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP);

            if (lstSP.Count() == 0)
            {
                return new HttpNotFoundResult();
            }

            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
          
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.LoaiSP = db.LoaiSanPhams.Single(n => n.MaLoaiSP == MaLoaiSP);

            //Phân trang
            int PageSize = 3;
            int PageNumber = (page ?? 1);

            return View(lstSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }
    }
}