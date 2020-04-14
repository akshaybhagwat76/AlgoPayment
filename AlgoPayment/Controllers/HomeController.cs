using AlgoPayment.Helpers;
using AlgoPayment.Models;
using AlgoPayment.VideModel;
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

        [HttpPost]
        public ActionResult UserPage(PaymentModel payment)
        {
            RemotePost myremotepost = new RemotePost();
            string key = "gtKFFx";
            string salt = "eCwWELxi";

            //posting all the parameters required for integration.

            myremotepost.Url = "https://test.payu.in/_payment";
            myremotepost.Add("key", "gtKFFx");
            string txnid =Generatetxnid();
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", "750");
            myremotepost.Add("productinfo", "Apple");
            myremotepost.Add("firstname", "akshay");
            myremotepost.Add("phone", "7383323830");
            myremotepost.Add("email", "akshaybhagwat76@gmail.com");
            myremotepost.Add("surl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Return/Return");//Change the success url here depending upon the port number of your local system.
            myremotepost.Add("furl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Return/Return");//Change the failure url here depending upon the port number of your local system.
            //myremotepost.Add("service_provider", "");
            //string hashString = key + "|" + txnid + "|" + "akshaybhagwat76@gmail.com" + " | " + "Apple" + "|" + "Apple" + "|" + "Apple@gmail.com" + "|||||||||||" + salt;
            string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||"+ salt;
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);

            return Content(myremotepost.Post(),System.Net.Mime.MediaTypeNames.Text.Html);

            //RemotePost myremotepost = new RemotePost();
            //NameValueCollection allKeys = ConfigurationManager.AppSettings;

            //using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            //{
            //    var user = db.UserDetails.Find(payment.CustomerID);

            //    myremotepost.Url = "https://test.payu.in/_payment";
            //    myremotepost.Add("key", allKeys["key"]);
            //    string txnid = new Common().Generatetxnid();
            //    myremotepost.Add("txnid", txnid);
            //    myremotepost.Add("amount", "750");
            //    myremotepost.Add("productinfo", payment.DeviceID);
            //    myremotepost.Add("firstname", user.Name);
            //    myremotepost.Add("phone", user.Mobile);
            //    myremotepost.Add("email", user.emailid);
            //    myremotepost.Add("surl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Return/Return");//Change the success url here depending upon the port number of your local system.
            //    myremotepost.Add("furl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Return/Return");//Change the failure url here depending upon the port number of your local system.
            //    myremotepost.Add("service_provider", "");
            //    string hashString = allKeys["key"] + "|" + txnid + "|" + 750 + "|" + payment.DeviceID + "|" + user.Name +","+user.Id.ToString() + "|" + user.emailid + "|||||||||||" + allKeys["SALT"];
            //    //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            //    string hash = new Common().Generatehash512(hashString);
            //    myremotepost.Add("hash", hash);

            //    myremotepost.Post();
            //}


            //bool isWeekDay = false;
            //if (payment != null)
            //{
            //    using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            //    {
            //        var algo = db.AlgoExpiries.FirstOrDefault(x => x.CustomerID == payment.CustomerID);
            //        if (algo == null)
            //        {
            //            algo = new AlgoExpiry();
            //            isWeekDay = true;
            //            algo.CustomerID = payment.CustomerID;
            //            algo.DeviceID = payment.DeviceID;
            //            algo.DateExpiry = DateTime.Now.AddDays(7).ToShortDateString();
            //            algo.AppName = "Default";
            //            algo.MaxUser = "1";
            //            db.AlgoExpiries.Add(algo);
            //        }
            //        else
            //        {
            //            algo.DateExpiry = DateTime.Now.AddMonths(1).ToShortDateString();
            //        }
            //        db.SaveChanges();
            //    }
            //}
            //return Json(myremotepost.Post(), JsonRequestBehavior.AllowGet);
        }
        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }


        public string Generatetxnid()
        {

            Random rnd = new Random();
            string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);

            return txnid1;
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

        #region Other methods
        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();


            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public string Post()
            {
                string paystring = string.Empty;
                System.Web.HttpContext.Current.Response.Clear();
                paystring += "<html><head>";

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                paystring += string.Format("</head><body onload=\"document.{0}.submit()\">", FormName);

                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                paystring += string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url);

                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    paystring += string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]);

                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                paystring += "</form>";

                System.Web.HttpContext.Current.Response.Write("</form>");
                paystring += "</body></html>";

                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
                return paystring;
            }
        }

        #endregion
    }
}