using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;

namespace ShoppingCartOnline.Controllers
{
    public class QuanLyPhanQuyenController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: QuanLyPhanQuyen
        public ActionResult Index()
        {
            return View(db.LoaiThanhViens.OrderBy(n => n.TenLoai));
        }

        [HttpGet]
        public ActionResult PhanQuyen(int? id)
        {
            if(id == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            LoaiThanhVien ltv = db.LoaiThanhViens.SingleOrDefault(n => n.MaLoaiTV == id);

            if(ltv == null)
            {
                return HttpNotFound();
            }

            //lấy danh sách quyền
            ViewBag.MaQuyen = db.Quyens;
            //lấy ra danh sách quyền của loại thành viên
            ViewBag.LoaiTVQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == id);
            return View(ltv);
        }

        [HttpPost]
        public ActionResult PhanQuyen(int? MaLoaiTV, IEnumerable<LoaiThanhVien_Quyen> lstPhanQuyen)
        {
            //trường hợp: phân quyền lại
            //1. Xóa quyền cũ
            var lstDaPhanQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == MaLoaiTV);
            if(lstDaPhanQuyen.Count() != 0)
            {
                db.LoaiThanhVien_Quyen.RemoveRange(lstDaPhanQuyen);
                db.SaveChanges();
            }

            if(lstPhanQuyen != null)
            {
                //kiểm tra danh sách quyền được check
                foreach (var item in lstPhanQuyen)
                {
                    item.MaLoaiTV = int.Parse(MaLoaiTV.ToString());
                    db.LoaiThanhVien_Quyen.Add(item);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}