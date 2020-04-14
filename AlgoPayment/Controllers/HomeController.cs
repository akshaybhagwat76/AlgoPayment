using AlgoPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlgoPayment.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SocialLogin(UserDetail data)
        {
            if (data != null)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    if (!IsUserExist(data.SocialId))
                    {
                        data.CreatedDate = DateTime.Now;

                        db.UserDetails.Add(data);
                        db.SaveChanges();
                    }
                }
            }
            return Json("Added", JsonRequestBehavior.AllowGet);
        }

        public bool IsUserExist(string id)
        {
            using (eponym_app_licenseEntities db =new eponym_app_licenseEntities())
            {
                return db.UserDetails.Count(x => x.SocialId == id) > 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult AddUser(UserDetail user)
        {
            if (user != null)
            {
                using (eponym_app_licenseEntities db= new eponym_app_licenseEntities())
                {
                    user.CreatedDate = DateTime.Now;
                    db.UserDetails.Add(user);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
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
    }
}