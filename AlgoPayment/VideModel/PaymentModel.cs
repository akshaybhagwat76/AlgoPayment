﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlgoPayment.VideModel
{
    public class PaymentModel
    {
        public string DeviceID { get; set; }
        public int CustomerID { get; set; }
        public string MaxUser { get; set; }
        public int Price { get; set; }
    }
}