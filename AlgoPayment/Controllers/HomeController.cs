using AlgoPayment.Helpers;
using AlgoPayment.Models;
using AlgoPayment.VideModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                    var user = db.UserDetails.Find(loggedInUser.Id);

                    if (algo != null)
                    {
                        Session["deviceID"] = algo.DeviceID;
                        Session["userId"] = algo.CustomerID;
                        Session["userInfo"] = JsonConvert.SerializeObject(user);
                        Session["existingUser"] = true;
                    }
                    else
                    {
                        Session["userInfo"] = JsonConvert.SerializeObject(user);
                        Session["userId"] = user.Id;
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

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Contents.RemoveAll();
            Session.Clear();
            return Json("Cleared", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Login(UserDetail data)
        {
            if (data != null)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    var user = db.UserDetails.Where(x => x.emailid == data.emailid && x.Password == data.Password).FirstOrDefault();
                    if (!user.IsEmailVerified)
                    {
                        return Json(new { data = "Account not email confirmed", status = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                    if (user != null)
                    {
                        Session["UserCredentials"] = new UserCredentials()
                        { emailid = user.emailid, Id = user.Id, Name = user.Name, Mobile = user.Mobile, City = user.City, State = user.State, SocialId = user.SocialId, Password = user.Password };
                        return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = "Invalid credentials", status = "Failed" }, JsonRequestBehavior.AllowGet);
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
                        data.IsEmailVerified = true;
                        db.UserDetails.Add(data);
                        db.SaveChanges();
                        int newCustomer = data.Id;
                        var user = db.UserDetails.Find(newCustomer);
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
                    else
                    {
                        var user = db.UserDetails.Where(x => x.SocialId == data.SocialId).FirstOrDefault();
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
            }
            return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
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
            bool Status = false;
            string message = "";
            if (user != null)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    user.CreatedDate = DateTime.Now;
                    db.UserDetails.Add(user);
                    if (1 == db.SaveChanges())
                    {
                        var messagedata = new
                        {
                            email = user.emailid,
                            url = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority,
                            token = System.Web.HttpContext.Current.Server.UrlEncode(Security.Encrypt(user.Id.ToString()))
                        };

                        MailManager mm = new MailManager();
                        String exMessage = mm.SendMail(user.emailid, Messages.ACCOUNT_ACTIVATION, string.Format(Messages.NEW_USERREGISTRATION_MESSAGE, messagedata.email, messagedata.url, messagedata.token));
                        if ("success" == exMessage)
                        {
                            message = "Registration successfully done. Account activation link " +
               " has been sent to your email id:" + user.emailid;
                            Status = true;
                        }
                        else
                        {
                            throw new Exception(exMessage);
                        }
                    }
                }
            }
            Session["Message"] = message;
            Session["Status"] = Status;

            return RedirectToAction("Index");
        }

    }
}