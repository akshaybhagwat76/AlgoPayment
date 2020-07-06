using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlgoPayment.Helpers
{
    public class Messages
    {
        public const string FAIL = "Fail";
        public const string SUCCESS = "Success";
        public const string ALREADY_EXISTS = "Already exists";

        public const string BAD_DATA = "Bad or Invalid data";
        public const string USER_EXISTS = "User already exists";
        public const string LOGIN_TYPE_FACEBOOK = "Facebook";
        public const string LOGIN_TYPE_LOCAL = "Local";

        public const string USER_ALREADY_REWARDED = "User is already rewarded";
        public const string USER_DONT_HAVE_ENOUGH_AMOUNT = "User have not wallet amount in his account";
        public const string INVALID_USER_PASS = "Invalid email or password";
        public const string FORGOTPASSEMAIL = "Invalid email.";
        public const string NOT_ACTIVE = "Account not active";
        public const string LESS_THEN_12_HOURS = "You are trying to update order after 12 hours, please contact to admin from message box regarding your order with order reference id {0}";

        public const string ERROR_SENDING_EMAIL = "Error sending email";

        public const string PASSWORD_RESET = "Reset Password Request";
        public const string PASSWORD_RESET_SUCCESS = "Please check your email for reset password";

        public const string PASSWORD_RESET_MESSAGE = "Hello,<br/><br/>We received a request to reset the password for your Account {0}.<br/><br/>Please click on the following link <a href= '{1}/Account/resetpassword?token={2}' target= '_blank' >{1}/Account/resetpassword?token={2}</a> <br/><br/>Please contact us if you have any problems with your login.<br/><br/>Thank you";

        public const string FORGET_PASSWORD_RESET_MESSAGE = "Hello,<br/><br/>We received a request to reset the password for your Account {0}.<br/><br/>Please click on the following link <a href= 'https://www.pithplay.com/lostpassword/{1}' target= '_blank' >https://www.pithplay.com/lostpassword/{1}</a> <br/><br/>For your security, this link will expire in 24 hours or after your password has been reset.<br/><br/>Please contact us if you have any problems with your login.<br/><br/>Thank you";


        public const string CURRENT_PASSWORD_MESSAGE = "Current password is wrong";

        public const string ADD_MESSAGE = "Successfully added {0} detail";

        public const string UPDATE_MESSAGE = "Successfully updated {0} detail";

        public const string DELETE_MESSAGE = "Successfully deleted {0} detail";

        public const string COMMENT_APPROVED_MESSAGE = "Successfully published {0} detail";

        public const string PRODUCT_IMAGE = "Product";

        public const string LOGO = "Logo";
        public const string Somethingwentwrong = "Something went wrong";


        public const string ORDER_NOT_REVISION_YET = "Order Not Delivered";


        public const string ABOUT_ORDER = "ABOUT YOUR ORDER";
        public const string ABOUT_ORDER_NO = "ABOUT YOUR ORDER {0}";
        public const string ORDER_NEW_NOTIFICATION_USER = "Sub-Order Id-{0}";
        public const string ORDER_UPDATE_NOTIFICATION_USER = "SUB- ABOUT YOUR ORDER ID--{0}";
        public const string ORDER_SUBMITTED_MESSAGE = "Hello,<br/><br/> We have received your order request. Our Admin will respond about your order very soon. <br/><br/> Thanks.";
        public const string ORDER_NEW_ADMIN_SUBMITTED_MESSAGE = "Hello,<br/><br/> We have received new order request id {0}. User : {1}. <br/><br/> Thanks.";

        public const string ORDER_UPDATED_MESSAGE = "Hello,<br/><br/> We have received your order request. Our Admin will respond about your order very soon. <br/><br/> Thanks.";
        public const string ORDER_COMPLETE_MESSAGE = "Hello,<br/><br/>Thank you for buying your product. <br/><br/> We have completed your order. <br/><br/> Thanks.";
        public const string ORDER_CANCEL_MESSAGE = "Hello,<br/><br/>We have cancelled your order as per your request. <br/><br/> Your $ {0} refund will be credited to dashboard as soon as possible in 3-4 bussiness day. <br/><br/> Thanks.";
        public const string ORDER_REFUNDED_MESSAGE = "Hello,<br/><br/>We have cancelled your order as per your request. <br/><br/> Your $ {0} refund credited to dashboard. <br/><br/> Thanks.";
        public const string ORDER_STATUS = "Hello,<br/><br/> We have received your order request. We have {0} your order. <br/><br/> Thanks.";
        public const string ORDER_UPDATED = "Order Updated of {0}";
        public const string ORDER_UPDATE_STATUS = "Hello,<br/><br/> We have received order update request order id of {0}. <br/><br/> Thanks.";
        public const string ORDER_UPDATESCRIPT_STATUS = "Hello,<br/><br/> We have received script updated order update request order id of {0}. <br/><br/> Please check previous file along with order id<br/><br/> Thanks.";
        public const string NOT_EMAIL_VERIFIED = "Account is not email verified";
        public const string NEW_USERREGISTRATION_MESSAGE = "Hello,<br/><br/>Thank you so much for registering with https://amibrokeralgo.in/ as {0}.<br/><br/>Please click on the following link to confirm your account link : <a href= '{1}/Account/confirmemail?token={2}' target= '_blank' >{1}/Account/confirmemail?token={2}</a> <br/><br/>Please contact us if you have any problems with your login.<br/><br/>Thank you";
        public const string ACCOUNT_ACTIVATION = "About your account activation from https://amibrokeralgo.in/";
    }
}