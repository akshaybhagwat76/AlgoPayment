using AlgoPayment.Helpers;
using AlgoPayment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlgoPayment.Controllers
{
    public class ReturnController : Controller
    {
        // GET: Return

        public ActionResult Index()
        {
            return View();
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
                    Response.Write(merc_hash_string);
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
                            var algo = db.AlgoExpiries.FirstOrDefault(x => x.CustomerID ==userId);
                            if (algo == null)
                            {
                                algo = new AlgoExpiry();
                                algo.CustomerID = userId;
                                algo.DeviceID = Request.Form["productinfo"];
                                algo.DateExpiry = DateTime.Now.AddDays(7).ToShortDateString();
                                algo.AppName = "Default";
                                algo.MaxUser = "1";
                                db.AlgoExpiries.Add(algo);
                            }
                            else
                            {
                                algo.DateExpiry = DateTime.Now.AddMonths(1).ToShortDateString();
                            }
                            db.SaveChanges();
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

        public ActionResult Fail()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
    }
}