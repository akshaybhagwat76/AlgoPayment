using AlgoPayment.Helpers;
using AlgoPayment.Models;
using AlgoPayment.VideModel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Razorpay.Api;
namespace AlgoPayment.Controllers
{
    public class HomeController : BaseController
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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", 5000); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("payment_capture", 1);

            RazorpayClient client = new RazorpayClient(ConfigurationManager.AppSettings["razorPayKey"], ConfigurationManager.AppSettings["razorPaySecret"]);
            Razorpay.Api.Order order = client.Order.Create(input);
            Session["razorPayOrderId"] = order["id"].ToString();

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


        public ActionResult ResellerFail()
        {
            return View();
        }

        public ActionResult ResellerSuccess()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(UserDetail data)
        {
            if (data != null)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    var user = db.UserDetails.Where(x => x.emailid == data.emailid && x.Password == data.Password).FirstOrDefault();
                    if (user != null && !user.IsEmailVerified)
                    {
                        return Json(new { data = "Account not email confirmed", status = "Failed" }, JsonRequestBehavior.AllowGet);
                    }
                    if (user != null)
                    {
                        CrossControllerSession["UserCredentials"] = new UserCredentials()
                        { emailid = user.emailid, Id = user.Id, Name = user.Name, UserRole = user.UserRole, Mobile = user.Mobile, City = user.City, State = user.State, SocialId = user.SocialId, Password = user.Password };
                        return Json(new { data = user, status = "Success" }, JsonRequestBehavior.AllowGet);
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
                        data.UserRole = "client";
                        db.UserDetails.Add(data);
                        db.SaveChanges();
                        int newCustomer = data.Id;
                        var user = db.UserDetails.Find(newCustomer);
                        if (user != null)
                        {
                            Session["UserCredentials"] = new UserCredentials()
                            { emailid = user.emailid, Id = user.Id, Name = user.Name, Mobile = user.Mobile, UserRole = user.UserRole, City = user.City, State = user.State, SocialId = user.SocialId, Password = user.Password };
                            return Json(new { data = user, status = "Success" }, JsonRequestBehavior.AllowGet);
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
                            { emailid = user.emailid, Id = user.Id, Name = user.Name, Mobile = user.Mobile, UserRole = user.UserRole, City = user.City, State = user.State, SocialId = user.SocialId, Password = user.Password };
                            return Json(new { data = user, status = "Success" }, JsonRequestBehavior.AllowGet);
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
                    user.UserRole = "client";
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

        public ActionResult AdminPage()
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var user = db.UserDetails.Where(x => x.UserRole.Contains("client")).ToList();
                var algo = db.AlgoExpiries.ToList();
                List<ClientViewModel> lst = new List<ClientViewModel>();

                foreach (var item in user)
                {
                    ClientViewModel obj = new ClientViewModel();
                    var pay = algo.Where(x => x.CustomerID == item.Id).FirstOrDefault();
                    obj.CustomerID = item.Id;
                    obj.AppName = pay != null ? pay.AppName : "N/A";
                    obj.DateExpiry = pay != null ? pay.DateExpiry : "N/A";
                    obj.DeviceID = pay != null ? pay.DeviceID : "N/A";
                    obj.CustomerName = item.Name;
                    obj.emailid = item.emailid;
                    obj.City = item.City;
                    obj.Password = item.Password;
                    obj.State = item.State;
                    obj.Mobile = item.Mobile;
                    lst.Add(obj);
                }

                ViewBag.lstClients = lst;
            }

            return View();
        }



        public ActionResult AdminResellers()
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var users = db.UserDetails.Where(x => x.UserRole == "reseller").ToList();
                var settings = db.AppSettings.ToList();
                var algo = db.AlgoExpiries.ToList();
                List<ResellerViewModel> lst = new List<ResellerViewModel>();

                foreach (var item in users)
                {
                    var auser = algo.Where(x => x.CustomerID == item.Id).FirstOrDefault();
                    var amount = settings.Where(x => x.ResellerId == item.Id).FirstOrDefault().Amount;
                    ResellerViewModel obj = new ResellerViewModel();
                    obj.CustomerID = item.Id;
                    obj.AppName = auser != null ? auser.AppName : "N/A";
                    obj.emailid = item.emailid;
                    obj.DateExpiry = auser != null ? auser.DateExpiry : "N/A";
                    obj.DeviceID = auser != null ? auser.DeviceID : "N/A";
                    obj.CustomerName = item.Name;
                    obj.City = item.City;
                    obj.Password = item.Password;
                    obj.State = item.State;
                    obj.Mobile = item.Mobile;
                    obj.ResellerAmount = amount;

                    //if (lst.Any(x => x.CustomerID != obj.CustomerID))
                    //{
                    lst.Add(obj);

                    //}
                }

                //var clients = (from n in db.AlgoExpiries
                //               from u in db.UserDetails
                //               from r in db.AppSettings
                //               where n.CustomerID == u.Id && r.ResellerId == u.Id && u.UserRole == "reseller"
                //               select new ResellerViewModel
                //               {
                //                   CustomerID = n.CustomerID,
                //                   AppName = n.AppName,
                //                   DateExpiry = n.DateExpiry,
                //                   DeviceID = n.DeviceID,
                //                   CustomerName = u.Name,
                //                   emailid = u.emailid,
                //                   City = u.City,
                //                   Password = u.Password,
                //                   State = u.State,
                //                   Mobile = u.Mobile,
                //                   ResellerAmount = r.Amount
                //               }).DistinctBy(x => x.CustomerID).ToList();
                ViewBag.lstResellers = lst;
            }

            return View();
        }

        [HttpPost]
        public ActionResult DeleteClient(int id)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                if (id > 0)
                {
                    var user = db.UserDetails.Find(id);
                    if (user != null)
                    {
                        try
                        {

                            db.UserDetails.Remove(user);
                            db.SaveChanges();
                            return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message.ToString());
                        }
                    }
                }
            }
            return Json(new { data = false, status = "Failed" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditResellerClient(int id)
        {
            try
            {
                ClientViewModel categoryVM = new ClientViewModel();

                if (id > 0)
                {

                    using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                    {

                        var client = (from n in db.AlgoExpiries
                                      from u in db.UserDetails
                                      where n.CustomerID == u.Id && u.Id == id && u.UserRole == "resellerclient"
                                      select new ClientViewModel
                                      {
                                          CustomerID = n.CustomerID,
                                          AppName = n.AppName,
                                          DateExpiry = n.DateExpiry,
                                          DeviceID = n.DeviceID,
                                          CustomerName = u.Name,
                                          emailid = u.emailid,
                                          City = u.City,
                                          MaxUser = n.MaxUser,
                                          Password = u.Password,
                                          State = u.State,
                                          Mobile = u.Mobile
                                      }).FirstOrDefault();
                        categoryVM = client;
                    }
                }
                return View(categoryVM);
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public ActionResult AddClientFromAdmin()
        {
            ClientViewModel categoryVM = new ClientViewModel();

            return View(categoryVM);
        }

        [HttpPost]
        public ActionResult MarkAsReseller(string param)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                if (!string.IsNullOrEmpty(param))
                {
                    var user = db.UserDetails.Find(int.Parse(param.Split(',')[0]));
                    if (user != null)
                    {
                        try
                        {
                            user.UserRole = "reseller";
                            var uid = Convert.ToInt32(param.Split(',')[0].ToString());
                            var setting = db.AppSettings.Where(x => x.ResellerId == uid).FirstOrDefault();
                            if (setting == null)
                            {
                                db.AppSettings.Add(new AppSetting { Amount = int.Parse(param.Split(',')[1]), ResellerId = int.Parse(param.Split(',')[0]) });
                            }
                            else
                            {
                                setting.Amount = int.Parse(param.Split(',')[1]);
                            }
                            db.SaveChanges();
                            return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message.ToString());
                        }
                    }

                }
            }
            return Json(new { data = false, status = "Failed" }, JsonRequestBehavior.AllowGet);
        }

        public bool IsUserExists(string email, int id)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                return db.UserDetails.Count(x => x.emailid == email && x.Id != id) > 0;
            }
        }

        public ActionResult EditClient(int id)
        {
            try
            {
                ClientViewModel categoryVM = new ClientViewModel();

                if (id > 0)
                {

                    ClientViewModel obj = new ClientViewModel();
                    using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                    {

                        var users = db.UserDetails.Where(x => x.Id == id).FirstOrDefault();
                        var algo = db.AlgoExpiries.ToList();
                        if (users != null)
                        {
                            var pay = algo.Where(x => x.CustomerID == users.Id).FirstOrDefault();
                            obj.CustomerID = users.Id;
                            obj.AppName = pay != null ? pay.AppName : "";
                            obj.DateExpiry = pay != null ? pay.DateExpiry : "";
                            obj.DeviceID = pay != null ? pay.DeviceID : "";
                            obj.MaxUser = pay != null ? pay.MaxUser : "";

                            obj.CustomerName = users.Name;
                            obj.emailid = users.emailid;
                            obj.City = users.City;
                            obj.Password = users.Password;
                            obj.State = users.State;

                            obj.Mobile = users.Mobile;
                        }

                        categoryVM = obj;
                    }
                }

                return View(categoryVM);
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult EditReseller(int id)
        {
            try
            {
                ResellerViewModel obj = new ResellerViewModel();
                if (id > 0)
                {


                    using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                    {

                        var users = db.UserDetails.Where(x => x.Id == id).FirstOrDefault();
                        var algo = db.AlgoExpiries.ToList();
                        var amount = db.AppSettings.FirstOrDefault(x => x.ResellerId == id).Amount;
                        if (users != null)
                        {
                            var pay = algo.Where(x => x.CustomerID == users.Id).FirstOrDefault();
                            obj.CustomerID = users.Id;
                            obj.AppName = pay != null ? pay.AppName : "";
                            obj.DateExpiry = pay != null ? pay.DateExpiry : "";
                            obj.DeviceID = pay != null ? pay.DeviceID : "";
                            obj.MaxUser = pay != null ? pay.MaxUser : "";

                            obj.CustomerName = users.Name;
                            obj.emailid = users.emailid;
                            obj.City = users.City;
                            obj.Password = users.Password;
                            obj.State = users.State;
                            obj.ResellerAmount = amount;
                            obj.Mobile = users.Mobile;
                        }
                    }
                }

                return View(obj);
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public int GetResellerAmount(int id)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                return db.AppSettings.Where(x => x.ResellerId == id).FirstOrDefault().Amount;
            }
        }
        public ActionResult ResellerPage()
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var loggedInUser = (UserCredentials)(Session["UserCredentials"]);
                if (loggedInUser != null)
                {
                    var amountUser = db.AppSettings.Where(x => x.ResellerId == loggedInUser.Id).FirstOrDefault().Amount;

                    var clients = (from n in db.AlgoExpiries
                                   from u in db.UserDetails
                                   where u.ResellerId == loggedInUser.Id && n.CustomerID == u.Id && u.UserRole == "resellerclient"
                                   select new ResellerViewModel
                                   {
                                       CustomerID = n.CustomerID,
                                       AppName = n.AppName,
                                       DateExpiry = n.DateExpiry,
                                       DeviceID = n.DeviceID,
                                       CustomerName = u.Name,
                                       emailid = u.emailid,
                                       City = u.City,
                                       MaxUser = n.MaxUser,
                                       Password = u.Password,
                                       State = u.State,
                                       ResellerAmount = amountUser,
                                       Mobile = u.Mobile
                                   }).DistinctBy(x => x.CustomerID).ToList();

                    ViewBag.lstClients = clients;
                }
            }
            return View();
        }



        public int GetResellerClients(int id)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                if (id > 0)
                {
                    return (from n in db.AlgoExpiries
                            from u in db.UserDetails
                            where u.ResellerId == id && n.CustomerID == id && u.UserRole == "resellerclient"
                            select u).ToList().Count();
                }
                return 0;
            }
        }

        public ActionResult ViewClient(int id)
        {

            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var usser = db.UserDetails.Find(id);
                if (usser != null)
                {
                    ViewBag.resellerName = usser.Name;
                }

                var clients = (from n in db.AlgoExpiries
                               from u in db.UserDetails
                               where u.ResellerId == id && n.CustomerID == u.Id && u.UserRole == "resellerclient"
                               select new ClientViewModel
                               {
                                   CustomerID = u.Id,
                                   AppName = n.AppName,
                                   DateExpiry = n.DateExpiry,
                                   DeviceID = n.DeviceID,
                                   MaxUser = n.MaxUser,
                                   CustomerName = u.Name,
                                   emailid = u.emailid,
                                   City = u.City,
                                   Password = u.Password,
                                   State = u.State,
                                   Mobile = u.Mobile,
                                   CreatedDate = u.CreatedDate
                               }).DistinctBy(x => x.CustomerID).ToList();
                ViewBag.lstResClients = clients;
            }

            return View();
        }

        public JsonResult UpdateClient(ClientViewModel categoryVM)
        {
            try
            {
                UserCredentials user1 = (UserCredentials)(Session["UserCredentials"]);
                if (categoryVM != null)
                {
                    if (!string.IsNullOrEmpty(categoryVM.emailid))
                    {
                        if (IsUserExists(categoryVM.emailid, categoryVM.CustomerID))
                        {
                            return Json(new { data = false, status = "Duplicate" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                            {
                                var user = db.UserDetails.Find(categoryVM.CustomerID);
                                if (user == null) { user = new UserDetail(); user.IsEmailVerified = true; user.ResellerId = user1.Id; user.CreatedDate = DateTime.Now; user.UserRole = "resellerclient"; }
                                user.City = categoryVM.City;
                                user.Password = categoryVM.Password;
                                user.State = categoryVM.State;
                                user.emailid = categoryVM.emailid;
                                user.Mobile = categoryVM.Mobile;
                                user.Name = categoryVM.CustomerName;
                                if (user.Id == 0)
                                {
                                    db.UserDetails.Add(user);
                                    db.SaveChanges();
                                }

                                var userId = categoryVM.CustomerID == 0 ? user.Id : categoryVM.CustomerID;
                                var algo = db.AlgoExpiries.Where(x => x.CustomerID == userId).FirstOrDefault();
                                if (algo == null) { algo = new AlgoExpiry(); }
                                if (algo != null)
                                {

                                    if (categoryVM.DateExpiry.Contains("-"))
                                    {
                                        categoryVM.DateExpiry= categoryVM.DateExpiry.Replace("-", "/");
                                    }

                                    DateTime date = DateTime.ParseExact(categoryVM.DateExpiry, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    DateTime? oldDate = null;
                                    if (!string.IsNullOrEmpty(algo.DateExpiry))
                                    {
                                        oldDate = DateTime.ParseExact(algo.DateExpiry, "dd-mm-yyyy", CultureInfo.InvariantCulture).Date;
                                        if (user1.UserRole == "reseller" && date.Date.Date < oldDate.Value.Date)
                                        {
                                            date = DateTime.ParseExact(algo.DateExpiry, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                        }
                                    }

                                    algo.DateExpiry = date.ToString("dd-MM-yyyy");
                                    algo.DeviceID = categoryVM.DeviceID;
                                    algo.MaxUser = categoryVM.MaxUser;
                                    algo.AppName = categoryVM.AppName;
                                    algo.CustomerID = userId;
                                }
                                if (algo.Id == 0)
                                {
                                    db.AlgoExpiries.Add(algo);
                                }

                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Json(new { data = true, status = "Success", error = "An Error Occurred" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { data = false, status = "Failed", error = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateReseller(ResellerViewModel categoryVM)
        {
            try
            {
                if (categoryVM != null)
                {
                    if (!string.IsNullOrEmpty(categoryVM.emailid))
                    {
                        if (IsUserExists(categoryVM.emailid, categoryVM.CustomerID))
                        {
                            return Json(new { data = false, status = "Duplicate" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                            {
                                var user = db.UserDetails.Find(categoryVM.CustomerID);
                                user.City = categoryVM.City;
                                user.Password = categoryVM.Password;
                                user.State = categoryVM.State;
                                user.emailid = categoryVM.emailid;
                                user.Mobile = categoryVM.Mobile;
                                user.Name = categoryVM.CustomerName;
                                user.ResellerId = categoryVM.CustomerID;

                                var algo = db.AlgoExpiries.Where(x => x.CustomerID == categoryVM.CustomerID).FirstOrDefault();
                                if (algo != null)
                                {
                                    DateTime date = DateTime.ParseExact(categoryVM.DateExpiry.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    algo.DateExpiry = date.ToString("dd-MM-yyyy");
                                    algo.DeviceID = categoryVM.DeviceID;
                                }
                                var setting = db.AppSettings.Where(x => x.ResellerId == categoryVM.CustomerID).FirstOrDefault();
                                if (setting != null)
                                {
                                    setting.Amount = categoryVM.ResellerAmount;
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Json(new { data = true, status = "Success", error = "An Error Occurred" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { data = false, status = "Failed", error = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AdminSetting()
        {
            return View();
        }

    }
}