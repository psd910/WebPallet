using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTinhTien.Authorize
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class RBACAuthorize : ActionFilterAttribute
    {
        public string TenNguoiDung { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["NguoiDung"] == null)
                filterContext.Result = new RedirectResult("~/Account/Login");
            var currentUser = HttpContext.Current.Session["NguoiDung"] as string;
            if (!string.IsNullOrEmpty(TenNguoiDung) && currentUser != TenNguoiDung)
                filterContext.Result = new RedirectResult("~/Home/Index");
        }
    }
}