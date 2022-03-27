using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApprovalPortal.Models;
using ApprovalPortal.Repository;
namespace ApprovalPortal.CustomFilter
{
    public class ExceptionHandler : FilterAttribute, IExceptionFilter
    {
        //private PPMSEntitie db = new PPMSEntitie();
        private readonly IApprovalRepo approvalRepo;
        public void OnException(ExceptionContext filterContext)
        {
           

            if (!filterContext.ExceptionHandled)
            {
                ExceptionLogger logger = new ExceptionLogger(filterContext.Exception.Message, "", filterContext.Exception.StackTrace.ToString(), System.DateTime.Now, Convert.ToString(filterContext.HttpContext.Session["PersonnelNo"]));
                logger.ExceptionMessage = filterContext.Exception.Message;
                logger.ExceptionStackTrace = filterContext.Exception.StackTrace;
                logger.ControllerName = filterContext.RouteData.Values["controller"].ToString();
                logger.PersonalNo = Convert.ToString(HttpContext.Current.Session["PersonnelNo"]);

                approvalRepo.LogError(logger);
                
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml"
                };

                filterContext.ExceptionHandled = true;
            }

        }
    }
}