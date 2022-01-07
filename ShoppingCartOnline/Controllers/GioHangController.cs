using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;

namespace ShoppingCartOnline.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        //Lấy giỏ hàng
        public List<ItemGioHang> LayGioHang()
        {
            //Giỏ hàng đã tồn tại
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if(lstGioHang == null)
            {
                //Nếu Session giỏ hàng chưa tồn tại thì khởi tạo giỏ hàng
                lstGioHang = new List<ItemGioHang>();
                Session["GioHang"] = lstGioHang;
            }

            return lstGioHang;
        }

        //Thêm giỏ hàng thông thường (Bằng cách load lại trang)
        public ActionResult ThemGioHang(int MaSP, string strURL)
        {
            //Kiểm tra sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if(sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Trường hợp 1: sản phẩm đã tồn tại trong giỏ hàng
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck != null)
            {
                //Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                if(sp.SoLuongTon<spCheck.SoLuong)
                {
                    return View("ThongBao");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                return Redirect(strURL);
            }

            //Trường hợp 2: sản phẩm chưa tồn tại trong giỏ hàng
            ItemGioHang itemGioHang = new ItemGioHang(MaSP);
            if (sp.SoLuongTon < itemGioHang.SoLuong)
            {
                return View("ThongBao");
            }
            lstGioHang.Add(itemGioHang);
            return Redirect(strURL);
        }

        //Tính tổng số lượng
        public double TinhTongSoLuong()
        {
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if(lstGioHang == null)
            {
                return 0;
            }

            return lstGioHang.Sum(n => n.SoLuong);
        }

        //Tính tổng tiền
        public decimal TinhTongTien()
        {
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }

            return lstGioHang.Sum(n => n.ThanhTien);
        }

        //Trả về partial view giỏ hàng kèm thông tin số lượng SP và tổng tiền SP
        public ActionResult GioHangPartial()
        {
            if(TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();

            return PartialView();
        }

        // GET: GioHang
        public ActionResult XemGioHang()
        {
            //Lấy các sản phẩm trong giỏ hàng để hiển thị ra view
            List<ItemGioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }


        //Chỉnh sửa giỏ hàng (không dùng Ajax)
        [HttpGet]
        public ActionResult SuaGioHang(int MaSP)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa
            if(Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Kiểm tra sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();

            //Kiểm tra sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if(spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Lấy list giỏ hàng tạo giao diện
            ViewBag.GioHang = lstGioHang;

            //Nếu giỏ hàng đã tồn tại
            return View(spCheck);
        }

        //Xử lý cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {
            //Kiểm tra số lượng tồn
            SanPham spCheck = db.SanPhams.SingleOrDefault(n => n.MaSP == itemGH.MaSP);
            if(spCheck.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }

            //Cập nhật số lượng trong session giỏ hàng
            List<ItemGioHang> lstGH = LayGioHang(); //1.lấy list SP hiện có trong session
            ItemGioHang itemGHUpdate = lstGH.Find(n => n.MaSP == itemGH.MaSP); //2.lấy sản phẩm cần cập nhật từ trong list
            itemGHUpdate.SoLuong = itemGH.SoLuong; //3.Cập nhật lại số lượng

            //4.Cập nhật lại giá tiền
            itemGHUpdate.ThanhTien = itemGHUpdate.SoLuong * itemGHUpdate.DonGia;

            return RedirectToAction("XemGioHang");
        }

        //Xử lý xóa sản phẩm trong giỏ hàng
        public ActionResult XoaGioHang(int MaSP)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Kiểm tra sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();

            //Kiểm tra sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //Xóa item trong giỏ hàng
            lstGioHang.Remove(spCheck);
            
            return RedirectToAction("XemGioHang");
        }
        
        //Xây dựng chức năng đặt hàng
        public ActionResult DatHang(KhachHang kh)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            KhachHang khachHang = new KhachHang();
            if(Session["TaiKhoan"] == null)
            {
                //Thêm khách hàng vào bảng khách hàng (đối với khách hàng chưa có tài khoản)
                khachHang = kh;
                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
            }
            else
            {
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                khachHang.TenKH = tv.HoTen;
                kh.DiaChi = tv.DiaChi;
                kh.Email = tv.Email;
                kh.SoDienThoai = tv.SoDienThoai;

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
            }

            //Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khachHang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            db.DonDatHangs.Add(ddh);
            db.SaveChanges();

            //Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();
            foreach(var item in lstGH)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
                db.ChiTietDonDatHangs.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XemGioHang");
        }

        //Thêm giỏ hàng bằng Ajax
        public ActionResult ThemGioHangAjax(int MaSP, string strURL)
        {
            //Kiểm tra sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }

            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Trường hợp 1: sản phẩm đã tồn tại trong giỏ hàng
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                //Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                if (sp.SoLuongTon < spCheck.SoLuong)
                {
                    return Content("<script> alert(\" Sản phẩm đã hết hàng \") </script>");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                ViewBag.TongSoLuong = TinhTongSoLuong();
                ViewBag.TongTien = TinhTongTien();
                return PartialView("GioHangPartial");
            }

            //Trường hợp 2: sản phẩm chưa tồn tại trong giỏ hàng
            ItemGioHang itemGioHang = new ItemGioHang(MaSP);
            if (sp.SoLuongTon < itemGioHang.SoLuong)
            {
                return Content("<script> alert(\" Sản phẩm đã hết hàng \") </script>");
            }
            lstGioHang.Add(itemGioHang);
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView("GioHangPartial");
        }
    }
}