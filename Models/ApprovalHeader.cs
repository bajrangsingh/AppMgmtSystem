using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApprovalPortal.Models
{
    public class ApprovalHeader
    {
        public int APP_ID { get; set; }

        [Required(ErrorMessage = "Please Enter Approval Title")]
        [MaxLength(255)]
        public string APP_ApprovalFor { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please Enter Approval Description")]
        public string APP_Description { get; set; }

        public string APP_DescriptionHtmlDecoded { get; set; }

        [Required(ErrorMessage = "Please Select Fiscal Year")]
        public int APP_FiscalYearId { get; set; }

        [Required(ErrorMessage = "Please Select Period")]
        public int APP_PeriodId { get; set; }
        [Required(ErrorMessage = "Please Select Function")]
        public int APP_FunctionId { get; set; }

        [Required(ErrorMessage = "Please Select TAG/Nature of Transaction")]
        public int APP_TAGNatureId { get; set; }

        public string APP_Creator { get; set; }

        [MaxLength(255)]
        //[Required(ErrorMessage = "Please Enter Remarks")]
        public string APP_CreatorRemarks { get; set; }


        public string APP_ReviewerID { get; set; }

        [Required(ErrorMessage = "Please Enter Reviewer Remarks")]
        [MaxLength(255)]
        public string APP_ReviewerRemarks { get; set; }

        [Required(ErrorMessage = "Please Select  Approval Trail")]
        public int APP_ApprovalTrailId { get; set; }

        [Required(ErrorMessage = "Please Select  Email List")]
        public int APP_ELId { get; set; }

        public int APP_StatusId { get; set; }
        public string APP_PendingWith { get; set; }
        public DateTime APP_CreatedOn { get; set; }

        public string FiscalYearText { get; set; }
        public string PeriodText { get; set; }
        public string FunctionText { get; set; }
        public string TNTText { get; set; }
        public object ReviewerName { get; set; }
        public string CreatorName { get; set; }

        public string ApprovalTrailName { get; set; }
        public string EL_EmailListName { get; set; }
        public string PendingWith { get; set; }
        public string ApprovalStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> App_Value { get; set; }

        public IList<Files> Attachments { set; get; }

        public string FirstLevelApprover { get; set; }

        [Required(ErrorMessage = "Please Select Action Radio Button")]
        public int ApproverAction { set; get; }
        public string Approver { set; get; }

        public IList<ApprovalMatrix> ApprovalMatrixList { get; set; }
        public IList<ApprovalTransaction> ApprovalTransactionList { get; set; }
        public IList<Files> UploadedAttachmentsList { get; set; }
        public IList<ApprovalNotes> ApprovalNotesList { get; set; }

        public bool IsActionable { get; set; }
        public string SendBackForRework { set; get; }

        public string SentBackToUser { get; set; }
        public bool IsSentBack { get; set; }

        [Required(ErrorMessage = "Please Enter Remarks")]
        public string ApproverRemarks { set; get; }

        public string ActionMethod { get; set; }

        public string UserRole { get; set; }

        //public string[] FinalEmailUsers { get; set; }

        public string[] SelectedaFinalEmailUsers_Entry { get; set; }
        public IList<FinalEmailUsersList> FinalEmailUsers { get; set; }

        public IList<FinalEmailUsersList> SelectedFinalEmailUsers { get; set; }

        public string Command { get; set; }

        public string MyAction { get; set; }
    }
    public class Files
    {
        public Nullable<int> FileID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        //public byte[] FileData { get; set; }
        public Nullable<int> APP_ID { get; set; }
        public string UploadedBy { get; set; }
        public string UploadedDate { get; set; }
        public string strUploadedDate { get; set; }

        public string EmployeeName { get; set; }

    }
    public class ApprovalList
    {
        public IList<ApprovalHeader> PendingRequests { get; set; }
        public IList<ApprovalHeader> OpenRequests { get; set; }
        public IList<ApprovalHeader> ClosedRequests { get; set; }
        public IList<ApprovalHeader> RejectedRequests { get; set; }
        public IList<ApprovalHeader> AllRequests { get; set; }
    }

}