using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApprovalPortal.Models
{
    public class Users
    {
        public int US_ID { get; set; }
        public string US_PersonnelNo { get; set; }
        public bool US_Creator { get; set; }
        public bool US_Reviewer { get; set; }
        public bool US_Admin { get; set; }
        public bool US_IsActive { get; set; }
        public string NameWithSAPID { get; set; }
        public string EmailWithSAPID { get; set; }
        public bool US_Approver { get; set; }

        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByPersonnelNo { get; set; }
        public string UpdatedByName { get; set; }
        public string UpdatedByPersonnelNo { get; set; }
    }

    public enum UserAction
    {
        Approve = 1,
        Reject = 2,
        Rework = 3
    }

    public class ApproverAction
    {
        public int AppID { get; set; }
        public string Approver { get; set; }
        public int Action { get; set; }
        public string Remarks { get; set; }
        public string SendBackTo { get; set; }
        public int APP_ELId { get; set; }

        public IList<FinalEmailUsersList> FinalEmailUsers { get; set; }
        public IList<FinalEmailUsersList> SelectedFinalEmailUsers { get; set; }
        public string[] SelectedaFinalEmailUsers_Entry { get; set; }

    }

}

