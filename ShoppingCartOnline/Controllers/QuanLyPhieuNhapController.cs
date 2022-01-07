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
    public class QuanLyPhieuNhapController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: QuanLyPhieuNhap
        [HttpGet]
        public ActionResult NhapHang()
        {
            ViewBag.ListNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;

            return View();
        }

        [HttpPost]
        public ActionResult NhapHang(PhieuNhap model, IEnumerable<ChiTietPhieuNhap> lstModel)
        {
            ViewBag.ListNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;
            //***chưa kiểm tra tất cả các dữ liệu đầu vào model
            db.PhieuNhaps.Add(model);
            db.SaveChanges(); //save changes để lấy được mã phiếu nhập gán cho lstChiTietPhieuNhap

            SanPham sp;

            foreach(var item in lstModel)
            {
                sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                sp.SoLuongTon += item.SoLuongNhap;
                item.MaPN = model.MaPN;
            }

            db.ChiTietPhieuNhaps.AddRange(lstModel);
            db.SaveChanges();

            return View();
        }

        [HttpGet]
        public ActionResult SanPhamHetHang()
        {
            //danh sách sản phẩm gần hết hàng (số lượng tồn <= 5)
            var lstSanPham = db.SanPhams.Where(n => n.SoLuongTon <= 5);

            return View(lstSanPham);
        }

        [HttpGet]
        public ActionResult NhapHangDon(int? id)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);

            ViewBag.ListNCC = db.NhaCungCaps;
            //Load NSX, LSP của sản phẩm
            ViewBag.NhaSanXuat = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == sp.MaNSX);
            ViewBag.LoaiSP = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == sp.MaLoaiSP);

            return View(sp);
        }

        [HttpPost]
        public ActionResult NhapHangDon(PhieuNhap pn, ChiTietPhieuNhap ctpn)
        {
            ViewBag.ListNCC = db.NhaCungCaps;
            //Load NSX, LSP của sản phẩm
            db.PhieuNhaps.Add(pn);
            db.SaveChanges();

            //Cập nhật tồn
            SanPham sp = db.SanPhams.Single(n => n.MaSP == ctpn.MaSP);
            sp.SoLuongTon += ctpn.SoLuongNhap;

            ctpn.MaPN = pn.MaPN;
            db.ChiTietPhieuNhaps.Add(ctpn);
            db.SaveChanges();

            return View(sp);
        }

        //giải phóng biến cho vùng nhớ
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}