using AlgoPayment.Helpers;
using AlgoPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlgoPayment.Controllers
{
    public class AccountController : Controller
    {
        private eponym_app_licenseEntities db = new eponym_app_licenseEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ConfirmEmail(string token)
        {
            ViewBag.Message = "";
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    string TokenDescrypt = Security.Decrypt(token);
                    int userId = 0;
                    Int32.TryParse(TokenDescrypt, out userId);

                    var objuser = db.UserDetails.Find(userId);
                    if (objuser != null)
                    {
                        if (IsUserExists(objuser.emailid))
                        {
                            bool confirmed = UpdateAccountActivation(objuser.Id);
                            if (confirmed)
                            {
                                Session["Message"] = "Email verified successfully";
                                Session["Status"] = true;
                                return Redirect("https://amibrokeralgo.in/Home/Index");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session["Message"] = ex.Message;
                }
            }
            return Redirect("https://amibrokeralgo.in/Home/Index");
        }

        public bool IsUserExists(string email)
        {
            return db.UserDetails.Count(x => x.emailid == email) > 0;
        }

        public bool UpdateAccountActivation(int id)
        {
            UserDetail user = new UserDetail();
            try
            {
                user = db.UserDetails.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    throw new Exception(Messages.BAD_DATA);
                }
                else
                {
                    user.IsEmailVerified = true;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

    }
}