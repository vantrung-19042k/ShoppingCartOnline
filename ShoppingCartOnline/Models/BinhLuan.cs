//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShoppingCartOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BinhLuan
    {
        public int MaBL { get; set; }
        public string NoiDungBL { get; set; }
        public Nullable<int> MaThanhVien { get; set; }
        public Nullable<int> MaSP { get; set; }
    
        public virtual SanPham SanPham { get; set; }
        public virtual ThanhVien ThanhVien { get; set; }
    }
}
