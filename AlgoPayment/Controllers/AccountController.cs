using AlgoPayment.Helpers;
using AlgoPayment.Models;
using AlgoPayment.VideModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AlgoPayment.Controllers
{
    public class AccountController : BaseController
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

        [HttpGet]
        public ActionResult ChangeCheckoutAmount(string method)
        {
            try
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings["PaymentOption"].Value = method;

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();
                return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { data = true, status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ChangeUserCheckoutAmount(int amount)
        {
            try
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings["UserSubscription"].Value = amount.ToString();

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();
                return Json(new { data = true, status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { data = true, status = "Failed" }, JsonRequestBehavior.AllowGet);
            }

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

        public ActionResult ForgotPassword()
        {
            return View();
        }
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
                {
                    var user = db.UserDetails.Where(o => o.emailid == model.Email).FirstOrDefault();
                    if (user == null)
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return View("ForgotPasswordConfirmation");
                    }
                    var messagedata = new
                    {
                        email = user.emailid,
                        url = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority,
                        token = System.Web.HttpContext.Current.Server.UrlEncode(Security.Encrypt(user.Id.ToString()))
                    };
                    MailManager mm = new MailManager();
                    String exMessage = mm.SendMail(user.emailid, Messages.PASSWORD_RESET, string.Format(Messages.PASSWORD_RESET_MESSAGE, messagedata.email, messagedata.url, messagedata.token));
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                ResetPasswordViewModel resetPassword = new ResetPasswordViewModel();
                resetPassword.Token = token;
                return View(resetPassword);
            }
            return Redirect("https://amibrokeralgo.in/Home/Index");
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string TokenDescrypt = Security.Decrypt(model.Token);
            int.TryParse(TokenDescrypt, out int userId);
            using (eponym_app_licenseEntities db = new eponym_app_licenseEntities())
            {
                var user = db.UserDetails.Find(userId);
                if (user!=null)
                {
                    user.Password = model.Password;
                    db.UserDetails.Attach(user);
                    db.Entry(user).Property(x => x.Password).IsModified = true;
                    db.SaveChanges();
                }            
            }
            return RedirectToAction("ResetPasswordConfirmation", "Account");
        }
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    }
}