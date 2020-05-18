using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlgoPayment.Controllers
{
    public class BaseController : Controller
    {
        private HttpSessionStateBase _session;
        protected HttpSessionStateBase CrossControllerSession
        {
            get
            {
                if (_session == null) _session = Session;
                return _session;
            }
            set
            {
                _session = Session;
            }
        }
    }
}