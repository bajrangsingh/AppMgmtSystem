using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApprovalPortal.Models
{
    public class ApprovalMatrix
    {
        public int AM_ID { get; set; }
        public Nullable<int> AM_APP_ID { get; set; }
        public Nullable<int> AM_ApprovingLevel { get; set; }
        public string AM_PersonnelNo { get; set; }
        public string AM_EmployeeName { get; set; }
        public string AM_Role { get; set; }
    }

    public class ApprovalTransaction
    {
        public int AT_ID { get; set; }
        public Nullable<int> AT_APP_ID { get; set; }
        public Nullable<int> AT_ApprovingLevel { get; set; }
        public string AT_PersonnelNo { get; set; }
        public int AT_StatusId{ get; set; }
        public string ApproverAction { get; set; }
        public string AT_Role { get; set; }
       // public DateTime? AT_ApprovingDate { get; set; }
        public string AT_ApprovingDate { get; set; }
        public string AT_ApprovingRemarks { get; set; }
        public string AT_EmployeeName { get; set; }
    }
    public class ApprovalNotes
    {
        public int AN_ID { get; set; }
        public int AN_APP_ID { get; set; } 
        public string AN_Notes { get; set; }
        public string AN_PersonnelNo { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AN_AddedOn { get; set; }
        public IEnumerable<ApprovalNotes> NotesList { get; set; }
    }
    public class ApprovalTrailUsersList
    {
        public int ATU_ID { get; set; }
        public int AT_ID { get; set; }
        public string ATU_PersonnelNo { get; set; }
        public int ATU_ApprovingLevel { get; set; }
        public bool ATU_IsActive { get; set; }
        public Nullable<decimal> ATU_ThresholdValue { get; set; }
        public string EmployeeName { get; set; }
        public string CreatedByPersonnelNo { get; set; }
        public string UpdatedByPersonnelNo { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string ApprovalTrailName { get; set; }
        public IEnumerable<ApprovalTrailUsersList> TrailUserList { get; set; }
    }
    public class FinalEmailUsersList
    {
        public int ELU_ID { get; set; }
        public string ELU_Email { get; set; }
        public bool ELU_IsActive { get; set; }
        public string EmployeeName { get; set; }
        public string EMail { get; set; }
    }
    public class ClassName
    {
        public int US_ID { get; set; }
        public int US_PersonnelNo { get; set; }
        public int US_Creator { get; set; }
        public int US_Reviewer { get; set; }
        public int US_Admin { get; set; }
        public bool US_Approver { get; set; }
        public bool US_IsActive { get; set; }
        public string NameWithSAPID { get; set; }
        public string EmailWithSAPID { get; set; }
    }

}