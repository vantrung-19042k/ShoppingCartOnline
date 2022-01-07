using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ShoppingCartOnline.Models;
using System.IO;
using PagedList;

namespace ShoppingCartOnline.Controllers
{
    public class QuanLySanPhamController : Controller
    {
        
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        // GET: QuanLySanPham
        public ActionResult Index(int? page)
        {
            IEnumerable<SanPham> lstSP = db.SanPhams;

            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }

            //thực hiện chức năng phân trang
            //1.tạo biến số sản phẩm trên trang
            int PageSize = 5;

            //2.tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);

            return View(lstSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }

        [HttpGet]
        public ActionResult ThemSanPham()
        {
            //Load dropdownlist NCC, NSX, LSP
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");

            return View();
        }

        [HttpPost]
        public ActionResult ThemSanPham(SanPham sp, HttpPostedFileBase HinhAnh)
        {
            //Load dropdownlist NCC, NSX, LSP
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");

            if(HinhAnh.ContentLength > 0) //kiểm tra hình ảnh đã được truyền vào chưa
            {

                var fileName = Path.GetFileName(HinhAnh.FileName); //lấy tên hình ảnh
                var path = Path.Combine(Server.MapPath("~/Content/images/SanPham"), fileName);
                if(System.IO.File.Exists(path))
                {
                    ViewBag.Upload = "Hình ảnh đã tồn tại";
                    return View();
                }
                else
                {
                    //Lấy hình ảnh đưa vào thư mục /images/SanPham
                    HinhAnh.SaveAs(path);
                    sp.HinhAnh = fileName;
                }
            }

            db.SanPhams.Add(sp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SuaSanPham(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if(id == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if(sp == null)
            {
                return HttpNotFound();
            }

            //Load dropdownlist NCC, NSX, LSP
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);

            return View(sp);
        }

        [HttpPost]
        public ActionResult SuaSanPham(SanPham sp)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);

            //chưa kiểm tra dữ liêu đầu vào, chỉ chạy khi nhập đúng
            db.Entry(sp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult XoaSanPham(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }

            //Load dropdownlist NCC, NSX, LSP
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);

            return View(sp);
        }

        [HttpPost]
        public ActionResult XoaSanPham(SanPham sp)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);

            //chưa kiểm tra dữ liêu đầu vào, chỉ chạy khi nhập đúng
            //chỉ mới xóa hình ảnh ở csdl chưa xóa trong thư mục

            SanPham spXoa = db.SanPhams.Single(n => n.MaSP == sp.MaSP);

            db.SanPhams.Remove(spXoa);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}