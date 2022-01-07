using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;

using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Web.Security;

namespace ShoppingCartOnline.Controllers
{
    public class HomeController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        
        // GET: /Home/
        public ActionResult Index()
        {
            //Lần lượt tạo các viewbag để lấy list sản phẩm từ csdl
            //List giày sneaker mới nhất
            var lstSneakerNew = db.SanPhams.Where(n => n.MaLoaiSP == 1 && n.Moi == 1).ToList();
            //Gán vào viewbag
            ViewBag.listSneakerNew = lstSneakerNew;

            //List quần áo mới nhất
            var lstQuanAoNew = db.SanPhams.Where(n => n.MaLoaiSP == 2 && n.Moi == 1).ToList();
            //Gán vào viewbag
            ViewBag.listQuanAoNew = lstQuanAoNew;

            //List phụ kiện mới nhất
            var lstPhuKienNew = db.SanPhams.Where(n => n.MaLoaiSP == 3 && n.Moi == 1).ToList();
            //Gán vào viewbag
            ViewBag.listPhuKienNew = lstPhuKienNew;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MenuPartial()
        {
            //Truy van lay ve 1 list san pham
            var lstSanPham = db.SanPhams;
            return PartialView(lstSanPham);
        }

        public ActionResult LoaiSanPhamPartial()
        {
            //Truy van lay ve 1 list loai san pham
            IEnumerable<LoaiSanPham> dsLoaiSP = db.LoaiSanPhams.ToList();

            return PartialView(dsLoaiSP);
        }

        [HttpGet] 
        public ActionResult DangKy()
        {
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(ThanhVien tv)
        {
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            //Kiểm tra captcha hợp lệ
            if (this.IsCaptchaValid("Captcha không hợp lệ"))
            {
                if(ModelState.IsValid)
                {
                    ViewBag.ThongBao = "Đăng ký thành công";
                    //Thêm khách hàng vào csdl
                    db.ThanhViens.Add(tv);
                    db.SaveChanges();
                }      
                return View();
            }

            ViewBag.ThongBao = "Sai mã captcha";

            return View();
        }

        public List<string> LoadCauHoi()
        {
            List<string> lstCauHoi = new List<string>();
            lstCauHoi.Add("Con vật mà bạn yêu thích?");
            lstCauHoi.Add("Ca sĩ mà bạn yêu thích?");
            lstCauHoi.Add("Món ăn bạn yêu thích?");

            return lstCauHoi;
        }

        //Xây dụng action đăng nhập (chưa phân quyền)
        //[HttpPost]
        //public ActionResult DangNhap(FormCollection f)
        //{
        //    string sTaiKhoan = f["txtTenDangNhap"].ToString();
        //    string sMatKhau = f["txtMatKhau"].ToString();

        //    ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan && n.MatKhau == sMatKhau);
        //    if(tv != null)
        //    {
        //        Session["TaiKhoan"] = tv;
        //        //return RedirectToAction("Index");
        //        return Content("<script>window.location.reload();</script>");
        //    }

        //    return Content("Tài khoản hoặc mật khẩu không đúng!!!");
        //}

        //public ActionResult DangXuat()
        //{
        //    Session["TaiKhoan"] = null;
        //    return RedirectToAction("Index");
        //}

        //Xây dụng action đăng nhập (có phân quyền)
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string sTaiKhoan = f["txtTenDangNhap"].ToString();
            string sMatKhau = f["txtMatKhau"].ToString();

            //kiểm tra và lấy thông tin thành viên
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan && n.MatKhau == sMatKhau);
            if (tv != null)
            {
                //lấy ra danh sách quyền của thành viên tương ứng với loại thành viên
                var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV ==  tv.MaLoaiTV);

                string Quyen = "";
                if (lstQuyen.Count() != 0)
                {
                    foreach (var item in lstQuyen)
                    {
                        Quyen += item.Quyen.MaQuyen + ",";
                    }


                    Quyen = Quyen.Substring(0, Quyen.Length - 1); //cắt dấu ','

                    PhanQuyen(tv.TaiKhoan.ToString(), Quyen);

                    Session["TaiKhoan"] = tv;

                    //return RedirectToAction("Index");
                    return Content("<script>window.location.reload();</script>");
                }
            }

            return Content("Tài khoản hoặc mật khẩu không đúng!!!");
        }

        public void PhanQuyen(string TaiKhoan, string Quyen)
        {
            FormsAuthentication.Initialize();

            var ticket = new FormsAuthenticationTicket(1,
                                TaiKhoan, //user
                                DateTime.Now, //begin
                                DateTime.Now.AddHours(3), //timeout
                                false, //remember?
                                Quyen, // permission .. 'admin', 'mod', ...
                                FormsAuthentication.FormsCookiePath
                );

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

            Response.Cookies.Add(cookie);
        }

        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        //tạo trang thông báo quyền truy cập
        public ActionResult ThongTinQuyen()
        {
            return View();
        }

    }
}