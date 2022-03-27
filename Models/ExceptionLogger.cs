using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApprovalPortal.Models
{
    public class ExceptionLogger
    {
        public int Id { get; set; }
        public string ExceptionMessage { get; set; }
        public string ControllerName { get; set; }
        public string ExceptionStackTrace { get; set; }
        public Nullable<System.DateTime> LogTime { get; set; }
        public string PersonalNo { get; set; }

        public ExceptionLogger(string _ExceptionMessage, string _ControllerName, string _ExceptionStackTrace, DateTime _LogTime, string _PersonalNo)
        {
            //Id = _Id;
            ExceptionMessage = _ExceptionMessage;
            ControllerName = _ControllerName;
            ExceptionStackTrace = _ExceptionStackTrace;
            LogTime = _LogTime;
            PersonalNo = _PersonalNo;
        }
    }
}