using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ApprovalPortal.Models
{
    public class Masters
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please Enter Value")]
        [MaxLength(255)]
        public string Text { get; set; }

        public bool IsActive { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedByName { get; set; }
        public string UpdatedOn { get; set; }
        public string CreatedByPersonnelNo { get; set; }
        public string UpdatedByPersonnelNo { get; set; }
        public string MasterName { get; set; }
    }
    public class FiscalYear:Masters
    {
    }

    public class Period : Masters
    {
    }

    public class Function : Masters
    {
    }
    public class EmailMessage
    {
        public string To { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public IList<Files> Attachments { get; set; }
    }
}