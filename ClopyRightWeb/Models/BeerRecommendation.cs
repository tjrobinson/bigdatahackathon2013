using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClopyRightCore;

namespace ClopyRightWeb.Models
{
    public class BeerRecommendation
    {
        public Beer Beer { get; set; }
        public Review Review { get; set; }
    }
}