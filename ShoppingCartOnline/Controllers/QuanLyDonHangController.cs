using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;
using System.Net.Mail;

namespace ShoppingCartOnline.Controllers
{
    public class QuanLyDonHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: QuanLyDonHang
        public ActionResult ChuaThanhToan()
        {
            //lấy danh sách các đơn hàng chưa duyệt
            var lst = db.DonDatHangs.Where(n => n.DaThanhToan == false).OrderBy(n => n.NgayDat);

            return View(lst);
        }

        public ActionResult ChuaGiao()
        {
            //lấy danh sách các đơn hàng chưa giao
            var lst = db.DonDatHangs.Where(n => n.TinhTrangGiaoHang == false).OrderBy(n => n.NgayDat);

            return View(lst);
        }

        public ActionResult DaGiaoDaThanhToan()
        {
            //lấy danh sách đơn hàng đã giao và đã thanh toán
            var lst = db.DonDatHangs.Where(n => n.TinhTrangGiaoHang == true && n.DaThanhToan == true).OrderBy(n => n.NgayDat);

            return View(lst);
        }

        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            //kiểm tra tính hợp lệ của id
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonDatHang ddh = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);

            //kiểm tra đơn hàng có tồn tại không
            if(ddh == null)
            {
                return HttpNotFound();
            }

            //lấy danh sách chi tiết đơn hàng để hiển thị cho người dùng
            var lstChiTietDonHang = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id);
            ViewBag.ListChiTietDonHang = lstChiTietDonHang;
            return View(ddh);
        }

        [HttpPost]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            //Cập nhật lại thông tin của đơn đặt hàng
            DonDatHang ddhUpdate = db.DonDatHangs.Single(n => n.MaDDH == ddh.MaDDH);
            ddhUpdate.DaThanhToan = ddh.DaThanhToan;
            ddhUpdate.TinhTrangGiaoHang = ddh.TinhTrangGiaoHang;
            db.SaveChanges();

            //lấy danh sách chi tiết đơn hàng để hiển thị cho người dùng
            var lstChiTietDonHang = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddhUpdate.MaDDH);
            ViewBag.ListChiTietDonHang = lstChiTietDonHang;

            //gửi khách hàng 1 mail để xác nhận việc thanh toán
            if (ddhUpdate.DaThanhToan == true && ddhUpdate.TinhTrangGiaoHang == true)
            {
                GuiMail("Xác nhận đơn hàng", "1851050165trung@ou.edu.vn", "vantrungtrollvl@gmail.com", "01662904532vantrung", "Đơn hàng của bạn đã được thanh toán thành công!!!");
            }

            return View(ddhUpdate);
        }

        public void GuiMail(string title, string toEmail, string fromEmail, string password, string content)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(toEmail); //mail nhận
            mail.From = new MailAddress(fromEmail); //mail gửi
            mail.Subject = title;
            mail.Body = content;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; //host gửi của mail
            smtp.Port = 587; //port của mail
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(fromEmail, password);
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}