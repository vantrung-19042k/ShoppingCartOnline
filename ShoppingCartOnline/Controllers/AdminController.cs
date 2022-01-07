using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;

namespace ShoppingCartOnline.Controllers
{
    public class AdminController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HeaderAdminPartial()
        {
            ViewBag.PageView = HttpContext.Application["PageView"].ToString(); //lấy số lượng người truy cập
            ViewBag.UserOnline = HttpContext.Application["UserOnline"].ToString(); //lấy số lượng người đang online

            ViewBag.TongDoanhThu = ThongKeDoanhThu();
            ViewBag.TongDonDatHang = ThongKeDonHang();
            ViewBag.TongNguoiDung = ThongKeNguoiDung();

            return PartialView();
        }

        public int ThongKeNguoiDung()
        {
            int tongNguoiDung = db.ThanhViens.Sum(n => n.MaThanhVien);

            return tongNguoiDung;
        }

        public decimal ThongKeDoanhThu()
        {
            //Thống kê theo tất cả doanh thu từ khi bắt đầu website
            decimal tongDoanhThu = db.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value;

            return tongDoanhThu;
        }

        public decimal ThongKeDoanhThuTheoThang(int thang, int nam)
        {
            //Thống kê theo tháng
            //Lấy ra những đơn đặt hàng có tháng và năm tương ứng
            var lstDDH = db.DonDatHangs.Where(n => n.NgayDat.Value.Month == thang && n.NgayDat.Value.Year == nam);
            decimal tongTien = 0;

            //duyệt đơn đặt hàng và tính tổng tiền tất cả sản phẩm trong đơn hàng đó
            foreach (var item in lstDDH)
            {
                tongTien += item.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value;
            }

            return tongTien;
        }

        public int ThongKeDonHang()
        {
            int ddh = db.DonDatHangs.Count();
            return ddh;
        }


        //giải phóng biến cho vùng nhớ
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}