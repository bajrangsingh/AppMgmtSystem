using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ApprovalPortal.CustomHelpers
{
    public static class HtmlExtensions
    {
        //public static IHtmlString Breadcrumb(this HtmlHelper helper, IPublishedContent thisPage)
        // {
        //     StringBuilder breadcrumb = new StringBuilder();

        //     IEnumerable<IPublishedContent> ancestors = thisPage.Ancestors().Reverse();

        //     foreach(IPublishedContent page in ancestors)
        //     {
        //         breadcrumb.Append($"<a href=\"{page.Url}\">{page.Name}</a>");
        //     }

        //     breadcrumb.Append($"<span>{thisPage.Name}</span>");

        //     return MvcHtmlString.Create(breadcrumb.ToString());
        // }
        public static IHtmlString GetFinancialYear(this HtmlHelper helper, DateTime dt)
        {
            string FinancialYear = string.Empty;
            FinancialYear fy = new FinancialYear(dt);
            FinancialYear = fy.ToString();
            return MvcHtmlString.Create(FinancialYear);
        }


    }
    public class FinancialYear
    {
        int yearNumber;
        private static readonly int firstMonthInYear = 4;

        public static FinancialYear Current
        {
            get { return new FinancialYear(DateTime.Today); }
        }

        public FinancialYear(DateTime forDate)
        {
            //forDate = new DateTime(2019, 4, 1);
            if (forDate.Month >= firstMonthInYear)
            {
                yearNumber = forDate.Year;
            }
            else
            {
                yearNumber = forDate.Year-1;
            }
        }

        public override string ToString()
        {
            return "FY-"+yearNumber.ToString() + "-" + (yearNumber + 1).ToString().Substring(2, 2);
        }
    }
}