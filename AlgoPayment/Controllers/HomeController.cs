﻿using AlgoPayment.Models;
using AlgoPayment.VideModel;
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
            var loggedInUser = (UserCredentials)(Session["UserCredentials"]);
            if (loggedInUser != null)
                return View("UserPage");
            return View();
        }


        public ActionResult UserPage()
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var loggedInUser = (UserCredentials)(Session["UserCredentials"]);
                if (loggedInUser != null)
                {
                    var algo = db.AlgoExpiries.Where(x => x.CustomerID == loggedInUser.Id).FirstOrDefault();
                    if (algo != null)
                    {
                        Session["deviceID"] = algo.DeviceID;
                        Session["userId"] = algo.CustomerID;
                        Session["existingUser"] = true;
                    }
                    else
                    {
                        Session["existingUser"] = false;
                    }
                }
                else
                {
                    Session["existingUser"] = false;
                }
            }
            return View();
        }

        public ActionResult PayNow(PaymentModel payment)
        {

            if (payment != null)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Contents.RemoveAll();
            Session.Clear();
            return View("Index");
        }

        [HttpPost]
        public JsonResult Login(UserDetail data)
        {
            if (data != null)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    var user = db.UserDetails.Where(x => x.emailid == data.emailid && x.Password == data.Password).FirstOrDefault();
                    if (user != null)
                    {
                        Session["UserCredentials"] = new UserCredentials()
                        { emailid = user.emailid, Id = user.Id, Name = user.Name, Mobile = user.Mobile, City = user.City, State = user.State, SocialId = user.SocialId, Password = user.Password };
                        return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = true, status = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { data = true, status = "Failed" }, JsonRequestBehavior.AllowGet);
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
                        data.Password = Guid.NewGuid().ToString("N").ToLower()
                      .Replace("1", "").Replace("o", "").Replace("0", "")
                      .Substring(0, 10);
                        db.UserDetails.Add(data);
                        db.SaveChanges();
                    }
                }
            }
            return Json("Added", JsonRequestBehavior.AllowGet);
        }

        public bool IsUserExist(string id)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
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
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
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