using AlgoPayment.Helpers;
using AlgoPayment.Models;
using AlgoPayment.VideModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
namespace AlgoPayment.Controllers
{
    public class ReturnController : BaseController
    {
        // GET: Return

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StorePayYouMoneyJson(string param)
        {
            HttpCookie cookie = new HttpCookie("payumoney");
            cookie.Expires = DateTime.Now.AddMinutes(10);
            cookie.Value = param;
            Response.Cookies.Add(cookie);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ResellerPayU(FormCollection form)
        {
            try
            {

                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (form[merc_hash_var] != null ? form[merc_hash_var] : "");

                    }
                    merc_hash = new Common().Generatehash512(merc_hash_string).ToLower();

                    if (merc_hash != form["hash"])
                    {

                        return RedirectToAction("ResellerFail", "Home");

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];
                        var nameWithID = Request.Form["firstname"];
                        var jsonUser = Request.Cookies["payumoney"].Value;
                        
                        using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                        {
                            var paymentList = JsonConvert.DeserializeObject<List<PaymentResponse>>(jsonUser);
                            foreach (var item in paymentList)
                            {
                                var algo1 = db.AlgoExpiries.FirstOrDefault(x => x.CustomerID == item.id);

                                if (algo1 != null)
                                {

                                    algo1.DateExpiry = Convert.ToDateTime(algo1.DateExpiry) < DateTime.Now ? DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy") : Convert.ToDateTime(algo1.DateExpiry).AddMonths(1).ToString("dd/MM/yyyy");
                                    db.SaveChanges();

                                }
                            }
                        }
                        ClearCookies();

                        return RedirectToAction("ResellerSuccess", "Home");

                        //Hash value did not matched
                    }

                }

                else
                {
                    ClearCookies();


                    return RedirectToAction("ResellerFail", "Home");

                }
            }

            catch (Exception ex)
            {
                ClearCookies();

                return RedirectToAction("ResellerFail", "Home");
            }


        }

        public static void ClearCookies()
        {

            if (System.Web.HttpContext.Current.Request.Cookies["payumoney"] != null)
            {
                HttpCookie aCookie = System.Web.HttpContext.Current.Request.Cookies["payumoney"];
                aCookie.Expires = DateTime.Now.AddDays(-10);
                aCookie.Value = "";
                System.Web.HttpContext.Current.Response.Cookies.Add(aCookie);
            }

        }
        [HttpGet]
        public ActionResult HandleResellerPayments(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                var paymentList = JsonConvert.DeserializeObject<List<PaymentResponse>>(param);
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    foreach (var item in paymentList)
                    {
                        var algo = db.AlgoExpiries.FirstOrDefault(x => x.CustomerID == item.id);

                        if (algo != null)
                        {

                            algo.DateExpiry = Convert.ToDateTime(algo.DateExpiry) < DateTime.Now ? DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy") : Convert.ToDateTime(algo.DateExpiry).AddMonths(1).ToString("dd/MM/yyyy");
                            db.SaveChanges();

                        }
                    }
                    return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { data = true, status = "Failed" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Return(FormCollection form)
        {
            try
            {

                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

                if (form["status"].ToString() == "success")
                {

                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();


                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (form[merc_hash_var] != null ? form[merc_hash_var] : "");

                    }
                    merc_hash = new Common().Generatehash512(merc_hash_string).ToLower();

                    if (merc_hash != form["hash"])
                    {

                        return View("Fail");

                    }
                    else
                    {
                        order_id = Request.Form["txnid"];
                        using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                        {
                            var nameWithID = Request.Form["firstname"];
                            var userId = Convert.ToInt32(nameWithID.Split(',')[1]);
                            var algo = db.AlgoExpiries.FirstOrDefault(x => x.CustomerID == userId);
                            if (algo == null)
                            {
                                algo = new AlgoExpiry() { CustomerID = userId, DeviceID = Request.Form["productinfo"], DateExpiry = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"), AppName = "Default", MaxUser = "1" };

                                db.AlgoExpiries.Add(algo);
                                db.SaveChanges();

                            }
                            else
                            {
                                algo.DateExpiry = Convert.ToDateTime(algo.DateExpiry) < DateTime.Now ? DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy") : Convert.ToDateTime(algo.DateExpiry).AddMonths(1).ToString("dd/MM/yyyy");
                                db.SaveChanges();

                            }
                        }
                        return View("Success");

                        //Hash value did not matched
                    }

                }

                else
                {
                    return View("Fail");
                }
            }

            catch (Exception ex)
            {
                return View("Fail");

            }


        }

        [HttpGet]
        public ActionResult HandleRazorPay(string deviceId)
        {
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var loggedInUser = (UserCredentials)(Session["UserCredentials"]);
                if (loggedInUser != null)
                {
                    var algo = db.AlgoExpiries.FirstOrDefault(x => x.CustomerID == loggedInUser.Id);
                    if (algo == null)
                    {
                        algo = new AlgoExpiry() { CustomerID = loggedInUser.Id, DeviceID = deviceId, DateExpiry = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"), AppName = "Default", MaxUser = "1" };

                        db.AlgoExpiries.Add(algo);
                        db.SaveChanges();
                        return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        algo.DateExpiry = Convert.ToDateTime(algo.DateExpiry) < DateTime.Now ? DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy") : Convert.ToDateTime(algo.DateExpiry).AddMonths(1).ToString("dd/MM/yyyy");
                        db.SaveChanges();
                        return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { data = true, status = "Failed" }, JsonRequestBehavior.AllowGet);

                }
            }
        }

        public ActionResult Fail()
        {
            var p = CrossControllerSession["UserCredentials"];
            CrossControllerSession["UserCredentials"] = CrossControllerSession["UserCredentials"];
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
    }
}