using ApprovalPortal.Models;
using ApprovalPortal.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ApprovalPortal.Controllers
{
    // [APActionFilter]
    public class ApprovalController : Controller
    {
        private readonly IApprovalRepo ApprovalRepo;
        public ApprovalController(IApprovalRepo _approvalRepo)
        {
            this.ApprovalRepo = _approvalRepo;
        }

        public IEnumerable<ApprovalHeader> getApprovalList(int? app_id)
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            return ApprovalRepo.GetApprovalList(app_id, loginEmp1.US_PersonnelNo, 0);
        }

        public ActionResult Create(int? app_id, bool IsActionable = true)
        {
            if (Session["loginEmp"] == null)
            {
                TempData["DisplayMsg"] = "Session Timeout, Please login again.";
                return RedirectToAction("Login", "Account");
            }

            var loginEmp1 = (Users)Session["loginEmp"];

            ApprovalHeader appHeader = null;
            if (app_id != null && app_id != 0)
            {
                // appHeader = getApprovalList(app_id).ToList().FirstOrDefault();
                appHeader = ApprovalRepo.GetApprovalAllData(Convert.ToInt32(app_id));
                Session["UploadedFiles"] = appHeader.UploadedAttachmentsList;
                appHeader.SelectedaFinalEmailUsers_Entry = GetDropdownListSelectedFinalEmailUsers(appHeader.SelectedFinalEmailUsers);
            }
            ViewBag.IsActionable = true;

            bool IsValidUser = true;
            if (app_id != 0)
            {
                if (loginEmp1.US_PersonnelNo != appHeader.APP_Creator)
                    IsValidUser = false;
            }

            if (!IsValidUser)
            {
                TempData["DisplayMsg"] = "Invalid Operation, Please login again.";
                return RedirectToAction("Login", "Account");
            }

            GetMasterViewBags();
            return View("create", appHeader);
        }
        public ActionResult Review(int app_id)
        {
            ApprovalHeader appHeader = ApprovalRepo.GetApprovalAllData(app_id);
            appHeader.APP_DescriptionHtmlDecoded = HttpUtility.HtmlDecode(appHeader.APP_Description);
            GetMasterViewBags();

            ViewBag.ActionMaster = Utility.GetActionMaster();
            ViewBag.SendBackForRework = GetBakUserList(appHeader);

            return View("Review", appHeader);
        }

        [HttpPost]
        public ActionResult Review(ApprovalHeader appHeader, string command = "")
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            appHeader.Approver = loginEmp1.US_PersonnelNo;
            Result output = null;
            output = ApprovalRepo.ReviewApproval(appHeader);

            if (output.Sucessful == 1)
            {
                string strDisplayMsg = "Approval No: " + Convert.ToString(output.ReturnVal) + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
                // Email email = factsheetService.GetFactsheetMail(output.ReturnVal);
                //SendMail(email);
            }
            else
            {
                //ApprovalRepo.LogExceptions(output, factsheetService);
                //throw ex;
                return View("Error", output);
            }
            // return RedirectToAction("Index");
            return RedirectToAction("ViewRequests", "Approval", new { ReqType = "Pending" });
        }

        [HttpPost]
        public ActionResult CreateNotes(ApprovalNotes note)
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            note.AN_PersonnelNo = loginEmp1.US_PersonnelNo;

            Result output = null;
            output = ApprovalRepo.SaveNote(note);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = "Note has been Added successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            return RedirectToAction("ViewNotes", new { app_id = note.AN_APP_ID });

        }

        public ActionResult Index()
        {
            if (Session["loginEmp"] != null)
            {
                Users user = (Users)Session["loginEmp"];
                if (string.IsNullOrEmpty(user.US_PersonnelNo))
                {
                    TempData["DisplayMsg"] = "Unathorised User, kindly contact Admin.";
                    return RedirectToAction("Login", "Account");
                }
                if (TempData["DisplayMsg"] != null)
                {
                    ViewBag.DisplayMsg = Convert.ToString(TempData["DisplayMsg"]);
                }
                ApprovalList allApprovalList = GetAllRecords();


                return View(allApprovalList);
            }
            else
            {
                // return RedirectToAction("Error");
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ShowApproval(int app_id, bool IsActionable, string UserRole)
        {
            if (Session["loginEmp"] == null)
            {
                TempData["DisplayMsg"] = "Session Timeout, Please login again.";
                return RedirectToAction("Login", "Account");
            }
            Users user = (Users)Session["loginEmp"];
            ApprovalHeader ObjApprovalHeader = new ApprovalHeader();
            ObjApprovalHeader = ApprovalRepo.GetApprovalAllData(app_id);
            ObjApprovalHeader.UserRole = UserRole;
            // ObjApprovalHeader.approvalMatrix = ObjApprovalHeader.ApprovalMatrix.OrderBy(x => x.OrderDate).ToList();
            // ObjApprovalHeader.approvalTransaction = ObjApprovalHeader.ApprovalMatrix.OrderBy(x => x.OrderDate).ToList();
            Session["UploadedFiles"] = ObjApprovalHeader.Attachments;
            GetMasterViewBags();
            // ViewBag.SendBackForRework = GetBakUserList(ObjApprovalHeader);
            ObjApprovalHeader.IsActionable = IsActionable;
            ViewBag.IsActionable = IsActionable;
            ViewBag.ActionMaster = Utility.GetActionMaster();
            ViewBag.SendBackForRework = GetBakUserList(ObjApprovalHeader);
            ObjApprovalHeader.SelectedaFinalEmailUsers_Entry = GetDropdownListSelectedFinalEmailUsers(ObjApprovalHeader.SelectedFinalEmailUsers);

            bool IsValidUser = false;
            if (user.US_PersonnelNo == ObjApprovalHeader.APP_Creator)
                IsValidUser = true;
            if (user.US_PersonnelNo == ObjApprovalHeader.APP_ReviewerID)
                IsValidUser = true;
            if (ObjApprovalHeader.ApprovalMatrixList.Where(x => x.AM_PersonnelNo == user.US_PersonnelNo).Count() > 0)
                IsValidUser = true;

            if (!IsValidUser && !user.US_Admin)
            {
                TempData["DisplayMsg"] = "Invalid Operation, Please login again.";
                return RedirectToAction("Login", "Account");
            }

            return View(ObjApprovalHeader); ;

        }

        public ActionResult ShowApprovalForCreator(int app_id, bool IsActionable, string UserRole)
        {
            if (Session["loginEmp"] == null)
            {
                TempData["DisplayMsg"] = "Session Timeout, Please login again.";
                return RedirectToAction("Login", "Account");
            }
            Users user = (Users)Session["loginEmp"];
            ApprovalHeader ObjApprovalHeader = new ApprovalHeader();
            ObjApprovalHeader = ApprovalRepo.GetApprovalAllData(app_id);
            ObjApprovalHeader.UserRole = UserRole;
            // ObjApprovalHeader.approvalMatrix = ObjApprovalHeader.ApprovalMatrix.OrderBy(x => x.OrderDate).ToList();
            // ObjApprovalHeader.approvalTransaction = ObjApprovalHeader.ApprovalMatrix.OrderBy(x => x.OrderDate).ToList();
            Session["UploadedFiles"] = ObjApprovalHeader.Attachments;
            GetMasterViewBags();
            // ViewBag.SendBackForRework = GetBakUserList(ObjApprovalHeader);
            ObjApprovalHeader.IsActionable = IsActionable;
            ViewBag.IsActionable = IsActionable;
            ViewBag.ActionMaster = Utility.GetActionMaster();

            ObjApprovalHeader.UserRole = UserRole;

            ViewBag.SendBackForRework = GetBakUserList(ObjApprovalHeader);
            ObjApprovalHeader.SelectedaFinalEmailUsers_Entry = GetDropdownListSelectedFinalEmailUsers(ObjApprovalHeader.SelectedFinalEmailUsers);

            bool IsValidUser = false;

            if (user.US_PersonnelNo == ObjApprovalHeader.APP_Creator)
                IsValidUser = true;


            if (!IsValidUser)
            {
                TempData["DisplayMsg"] = "Invalid Operation, Please login again.";
                return RedirectToAction("Login", "Account");
            }

            return View(ObjApprovalHeader); ;

        }

        public ActionResult ViewNotes(int app_id, int? AN_ID)
        {
            try
            {
                var NotesList = getNotes(app_id, AN_ID);
                ApprovalNotes note = new ApprovalNotes();
                note.AN_APP_ID = app_id;
                note.NotesList = NotesList;
                if (AN_ID != null)
                {
                    note.AN_Notes = NotesList.FirstOrDefault().AN_Notes;
                    note.AN_ID = Convert.ToInt32(AN_ID);
                    note.AN_APP_ID = app_id;
                }

                return View("ViewNotes", note);
            }
            catch (Exception ex)
            {
                HandleErrorInfo hei = new HandleErrorInfo(ex, "Approver", "ViewNotes");
                return View("Error", hei);
            }
        }
        public ActionResult DeleteNotes(int app_id, int? AN_ID)
        {
            Result output = ApprovalRepo.DeleteNote(Convert.ToInt32(AN_ID));
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = "Note has been Deleted successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            return RedirectToAction("ViewNotes", new { app_id = app_id });
        }

        private IEnumerable<ApprovalNotes> getNotes(int app_id, int? AN_ID)
        {
            return ApprovalRepo.GetNotesList(app_id, AN_ID);
        }

        public List<System.Web.Mvc.SelectListItem> GetBakUserList(ApprovalHeader objAppHeader)
        {
            List<SelectListItem> listApprovers = new List<SelectListItem>();
            if (Session["loginEmp"] != null)
            {
                Users user = (Users)Session["loginEmp"];
                int? currentUSerlevel = objAppHeader.ApprovalMatrixList.Where(x => x.AM_PersonnelNo == user.US_PersonnelNo).Select(x => x.AM_ApprovingLevel).FirstOrDefault();
                var approvalMatrix = objAppHeader.ApprovalMatrixList.Where(x => x.AM_ApprovingLevel < currentUSerlevel).OrderBy(x => x.AM_ApprovingLevel).ToList();

                foreach (ApprovalMatrix item in approvalMatrix)
                {
                    listApprovers.Add(new SelectListItem { Text = item.AM_EmployeeName, Value = item.AM_PersonnelNo.ToString(), });
                }
            }
            return listApprovers;
        }

        public ActionResult ViewRequests(string ReqType)
        {

            if (Session["loginEmp"] == null)
            {
                TempData["DisplayMsg"] = "Session Timeout, Please login again.";
                return RedirectToAction("Login", "Account");
            }
            ApprovalList allApprovalList = GetAllRecords();
            switch (ReqType.ToUpper())
            {
                case "OPEN":
                    return View("ViewRequests", allApprovalList.OpenRequests);

                case "PENDING":
                    return View("ViewRequests", allApprovalList.PendingRequests);

                case "CLOSED":
                    return View("ViewRequests", allApprovalList.ClosedRequests);

                case "REJECTED":
                    return View("ViewRequests", allApprovalList.RejectedRequests);

                default:
                    return View("ViewRequests", allApprovalList.OpenRequests);
            }


        }


        /*public ActionResult DownloadDoument(int? FileID)
        {
            if (Session["UploadedFiles"] != null)
            {
                var UploadedFile = (List<Files>)Session["UploadedFiles"];
                var DocumentInfo = UploadedFile.Where(x => x.FileID == FileID).First();
                string filename = DocumentInfo.FileName;
                //byte[] filedata = DocumentInfo.FileData;
                string contentType = MimeMapping.GetMimeMapping(DocumentInfo.FileType);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = filename,
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(filedata, contentType);
            }
            return null;

        }*/


        private ApprovalList GetAllRecords()
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            //ViewBag.ManagerData = loginEmp1.ManagerName;


            var allGridData = ApprovalRepo.GetApprovalList(null, loginEmp1.US_PersonnelNo, 0).OrderByDescending(app => app.APP_ID).ToList();
            ApprovalList allRequestList = new ApprovalList();

           // allRequestList.PendingRequests = allGridData.Where(x => x.MyAction == "Create" || x.MyAction == "Review" || x.MyAction == "Approve").ToList();
            allRequestList.PendingRequests = allGridData.Where(x => x.MyAction == "Create" || x.MyAction == "Review" || x.MyAction == "Approve").ToList();
            //CreateView
            
          //  allRequestList.PendingRequests.Where(x => x.MyAction == "Create").ToList().ForEach(c => c.ActionMethod = "Create");

            allRequestList.PendingRequests.Where(x => x.MyAction == "Create").ToList().ForEach(c => c.ActionMethod = "ShowApprovalForCreator");

            allRequestList.PendingRequests.Where(x => x.MyAction == "Review").ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.PendingRequests.Where(x => x.MyAction == "Review").ToList().ForEach(c => c.UserRole = "Reviewer");

            allRequestList.PendingRequests.Where(x => x.MyAction == "Approve").ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.PendingRequests.Where(x => x.MyAction == "Approve").ToList().ForEach(c => c.UserRole = "Approver");

            allRequestList.PendingRequests.ToList().ForEach(c => c.IsActionable = true);


            allRequestList.OpenRequests = allGridData.Where(x => x.MyAction == "View" && x.APP_PendingWith != null && x.PendingWith != "Rejected" && x.PendingWith != "All Approved").ToList();
            allRequestList.OpenRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.OpenRequests.ToList().ForEach(c => c.IsActionable = false);


            allRequestList.ClosedRequests = allGridData.Where(x => x.MyAction == "View" && x.PendingWith == "All Approved").ToList();
            allRequestList.ClosedRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.ClosedRequests.ToList().ForEach(c => c.IsActionable = false);

            allRequestList.RejectedRequests = allGridData.Where(x => x.MyAction == "View" && x.PendingWith == "Rejected").ToList();
            allRequestList.RejectedRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.RejectedRequests.ToList().ForEach(c => c.IsActionable = false);

            //allRequestList.AllRequests = allGridData;
            //allRequestList.AllRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            //allRequestList.AllRequests.ToList().ForEach(c => c.IsActionable = false);


            return allRequestList;
        }

        private ApprovalList GetAllRecords_bkp()
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            //ViewBag.ManagerData = loginEmp1.ManagerName;
            var allGridData = ApprovalRepo.GetApprovalList(null, loginEmp1.US_PersonnelNo, 0).OrderByDescending(app => app.APP_ID).ToList();
            ApprovalList allRequestList = new ApprovalList();

            if (loginEmp1.US_Creator == true)
            {
                allRequestList.PendingRequests = allGridData.Where(app => app.APP_PendingWith.ToUpper() == loginEmp1.US_PersonnelNo.ToUpper()).OrderByDescending(app => app.APP_ID).ToList();
                allRequestList.PendingRequests.ToList().ForEach(c => c.ActionMethod = "Create");

                allRequestList.OpenRequests = allGridData.Where(app => app.APP_PendingWith.ToUpper() != loginEmp1.US_PersonnelNo.ToUpper() && app.ApprovalStatus != "APPROVED" && app.ApprovalStatus.ToUpper() != "REJECTED").OrderByDescending(app => app.APP_ID).ToList();
                allRequestList.OpenRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
                allRequestList.OpenRequests.ToList().ForEach(c => c.IsActionable = false);
            }
            else if (loginEmp1.US_Reviewer == true)
            {
                allRequestList.PendingRequests = allGridData.Where(app => app.APP_PendingWith == loginEmp1.US_PersonnelNo).OrderByDescending(emp => emp.APP_ID).ToList();
                allRequestList.PendingRequests.Where(x => x.APP_StatusId <= 2).ToList().ForEach(c => c.ActionMethod = "ShowApproval");//    "Review");
                allRequestList.PendingRequests.Where(x => x.APP_StatusId <= 2).ToList().ForEach(c => c.UserRole = "Reviewer");

                allRequestList.PendingRequests.Where(x => x.APP_StatusId > 2).ToList().ForEach(c => c.ActionMethod = "ShowApproval");
                allRequestList.PendingRequests.ToList().ForEach(c => c.IsActionable = true);

                allRequestList.OpenRequests = allGridData.Where(app => app.APP_ReviewerID == loginEmp1.US_PersonnelNo && app.APP_PendingWith != loginEmp1.US_PersonnelNo && app.ApprovalStatus.ToUpper() != "CLOSED" && app.ApprovalStatus.ToUpper() != "REJECTED").OrderByDescending(app => app.APP_ID).ToList();
                allRequestList.OpenRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
                allRequestList.OpenRequests.ToList().ForEach(c => c.IsActionable = false);
            }
            else if (loginEmp1.US_Approver == true)
            {
                allRequestList.PendingRequests = allGridData.Where(app => app.APP_PendingWith == loginEmp1.US_PersonnelNo).OrderByDescending(emp => emp.APP_ID).ToList();
                allRequestList.PendingRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
                allRequestList.PendingRequests.Where(x => x.APP_StatusId >= 2).ToList().ForEach(c => c.UserRole = "Approver");
                allRequestList.PendingRequests.ToList().ForEach(c => c.IsActionable = true);

                //allRequestList.OpenRequests = allGridData.Where(app => app.approvalMatrix.Where(x=>x.AM_PersonnelNo==loginEmp1.US_PersonnelNo).Count()>0   && app.APP_PendingWith != loginEmp1.US_PersonnelNo && app.ApprovalStatus.ToUpper() != "CLOSED" && app.ApprovalStatus.ToUpper() != "REJECTED").OrderByDescending(app => app.APP_ID).ToList();
                allRequestList.OpenRequests = allGridData.Where(app => app.APP_PendingWith != loginEmp1.US_PersonnelNo && app.ApprovalStatus.ToUpper() != "CLOSED" && app.ApprovalStatus.ToUpper() != "REJECTED").OrderByDescending(app => app.APP_ID).ToList();
                allRequestList.OpenRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
                allRequestList.OpenRequests.ToList().ForEach(c => c.IsActionable = false);


            }




            // allRequestList.ClosedRequests = allGridData.Where(emp => emp.CreatedBy == loginEmp1.PersonnelNo && emp.Status.ToUpper() != loginEmp1.EmployeeName.ToUpper() && emp.Status.ToUpper() == "CLOSED").OrderByDescending(emp => emp.Factsheet_ID).ToList();
            allRequestList.ClosedRequests = allGridData.Where(emp => emp.APP_PendingWith.ToUpper() != loginEmp1.US_PersonnelNo.ToUpper() && emp.ApprovalStatus.ToUpper() == "ALL APPROVAL DONE").OrderByDescending(app => app.APP_ID).ToList();
            allRequestList.ClosedRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.ClosedRequests.ToList().ForEach(c => c.IsActionable = false);

            //allRequestList.RejectedRequests = allGridData.Where(app => app.APP_Creator == loginEmp1.US_PersonnelNo && app.APP_PendingWith.ToUpper() != loginEmp1.US_PersonnelNo.ToUpper() && app.ApprovalStatus.ToUpper() == "REJECTED").OrderByDescending(app => app.APP_ID).ToList();
            allRequestList.RejectedRequests = allGridData.Where(emp => emp.APP_PendingWith.ToUpper() != loginEmp1.US_PersonnelNo.ToUpper() && emp.ApprovalStatus.ToUpper() == "REJECTED").OrderByDescending(app => app.APP_ID).ToList();
            allRequestList.RejectedRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.RejectedRequests.ToList().ForEach(c => c.IsActionable = false);

            return allRequestList;
        }
        [HttpPost]
        public async Task<ActionResult> Create(ApprovalHeader appHeader, string command = "")
        {

            if (Session["loginEmp"] == null)
            {
                TempData["DisplayMsg"] = "Session Timeout, Please login again.";
                return RedirectToAction("Login", "Account");
            }
            Result output = null;
            var loginEmp = (Users)Session["loginEmp"];
            appHeader.APP_Creator = loginEmp.US_PersonnelNo;
            if (command.ToLower() == "draft")
            {
                appHeader.APP_StatusId = 0;
            }
            else
            {
                appHeader.APP_StatusId = 2;
            }
            if (Request.Files.Count > 0)
            {
                appHeader.Attachments = GetUploadedFiles();
            }
            output = ApprovalRepo.SaveApproval(appHeader);

            if (output.Sucessful == 1)
            {
                string strDisplayMsg = "Approval No: " + Convert.ToString(output.ReturnVal) + " is Created/Updated successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
                // Email email = factsheetService.GetFactsheetMail(output.ReturnVal);
                //SendMail(email);
                SaveAttachments(output.ReturnVal);
                EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(Convert.ToInt32(output.ReturnVal));
                //Utility.SendMailThroughSendGrid(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

                //SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);
                await Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

            }
            else
            {
                //ApprovalRepo.LogExceptions(output, factsheetService);
                //throw ex;
                TempData["ErrorMSg"] = output.ShortMsg;
                HandleErrorInfo hei = new HandleErrorInfo(output.exception, "Create", "Approval");
                //return View("Error", hei);
                return RedirectToAction("Index");
            }
           // return RedirectToAction("Index");
            return RedirectToAction("ViewRequests", "Approval", new { ReqType = "Pending" });
        }

        // public async Task<ActionResult> ShowApprovalForCreator(ApprovalHeader objApprovalHeader)
        [HttpPost]
        public async Task<ActionResult> ApproveFromCreator(ApprovalHeader objApprovalHeader)
        {
            //UpdateRequestStatusFromCreator
            Result output = null;
            try
            {
                if (Session["loginEmp"] != null)
                {
                    Users LoggedinUser = (Users)Session["loginEmp"];
                    objApprovalHeader.Approver = LoggedinUser.US_PersonnelNo;
                    if (objApprovalHeader.APP_PendingWith != LoggedinUser.US_PersonnelNo)
                    {
                        ViewBag.Message = "Invalid action.";
                        TempData["ErrorMsg"] = "Invalid action.";
                        return RedirectToAction("Index");
                    }

                    if (string.IsNullOrEmpty(objApprovalHeader.Approver))
                    {
                        var loginEmp1 = (Users)Session["loginEmp"];
                        string approvalBy = loginEmp1.US_PersonnelNo;
                        objApprovalHeader.Approver = approvalBy;
                    }


                    ApproverAction ObjApproverAction = new ApproverAction();
                    ObjApproverAction.AppID = Convert.ToInt32(objApprovalHeader.APP_ID);
                    ObjApproverAction.Action = Convert.ToInt32(objApprovalHeader.ApproverAction);
                    ObjApproverAction.Approver = objApprovalHeader.Approver;
                    ObjApproverAction.Remarks = objApprovalHeader.ApproverRemarks;
                    ObjApproverAction.SelectedaFinalEmailUsers_Entry = objApprovalHeader.SelectedaFinalEmailUsers_Entry;

                    output = ApprovalRepo.UpdateRequestStatusFromCreator(ObjApproverAction);
                    if (output.Sucessful == 1)
                    {
                        TempData["DisplayMsg"] = "Approval  ID: " + objApprovalHeader.APP_ID + " has been processed.";
                        EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(objApprovalHeader.APP_ID);
                        await Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

                        return RedirectToAction("ViewRequests", "Approval", new { ReqType = "Pending" });
                    }
                    else
                    {
                        Utility.LogExceptions(output);
                        return View("Error", output);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
                return Json(new { status = "failure", message = ex.Message });
            }
        }

        private void SaveAttachments(string approvalId)
        {
            int NoOfFiles = Request.Files.Count;
            for (int i = 0; i < NoOfFiles; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file.FileName != "")
                {
                    var fileName = approvalId + "_" + Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Attachments"), fileName);
                    file.SaveAs(path);

                    //i++;
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Approve(ApprovalHeader objApprovalHeader, string command)
        {
            Result output = null;
            try
            {
                if (Session["loginEmp"] != null)
                {
                    Users LoggedinUser = (Users)Session["loginEmp"];
                    objApprovalHeader.Approver = LoggedinUser.US_PersonnelNo;
                    if (objApprovalHeader.APP_PendingWith != LoggedinUser.US_PersonnelNo)
                    {
                        ViewBag.Message = "Invalid action.";
                        TempData["ErrorMsg"] = "Invalid action.";
                        return RedirectToAction("Index");
                    }


                    if (command == "Review") // this is for Reviewer
                    {

                        output = ApprovalRepo.ReviewApproval(objApprovalHeader);
                    }
                    else if (command == "Submit") // this is for approver
                    {
                        output = UpdateRequestStatus(objApprovalHeader);
                    }
                    if (output.Sucessful == 1)
                    {
                        TempData["DisplayMsg"] = "Approval  ID: " + objApprovalHeader.APP_ID + " has been processed.";
                        EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(objApprovalHeader.APP_ID);

                        // Utility.SendMailThroughSendGrid(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);
                        // Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

                        // SendMail(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);
                        //SendMail(EmailToSend);
                        //await this.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

                        await Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

                        return RedirectToAction("ViewRequests", "Approval", new { ReqType = "Pending" });
                    }
                    else
                    {
                        Utility.LogExceptions(output);
                        return View("Error", output);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
                return Json(new { status = "failure", message = ex.Message });
            }
            //}
            //return RedirectToAction("ShowApproval", "Approval", new { app_id = objApprovalHeader.APP_ID, IsActionable = true });
        }


        [AllowAnonymous]
        public async Task<ActionResult> ApproveOnMail(string App_ID, string command, string PersonnelNo)
        {
            ApprovalHeader objApprovalHeader = ApprovalRepo.GetApprovalAllData(Convert.ToInt32(App_ID));
            objApprovalHeader.ApproverRemarks = "Approved on Mail";
            objApprovalHeader.ApproverAction = 1;
            if (objApprovalHeader.APP_PendingWith == PersonnelNo) // to handle mutiple mail approval for same user
            {
                UpdateRequestOnEmail(command, PersonnelNo, objApprovalHeader);
                ViewBag.Message = "Approval ID:" + App_ID + " Processed Successfully.";

                EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(Convert.ToInt32(App_ID));
                await Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

            }
            else
            {
                ViewBag.Message = "Invalid action.";
            }


            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> RejectOnMail(string App_ID, string command, string PersonnelNo)
        {
            ApprovalHeader objApprovalHeader = ApprovalRepo.GetApprovalAllData(Convert.ToInt32(App_ID));
            objApprovalHeader.ApproverRemarks = "Rejected on Mail";
            objApprovalHeader.ApproverAction = 2;
            if (objApprovalHeader.APP_PendingWith == PersonnelNo)// to handle mutiple mail approval for same user
            {
                objApprovalHeader.IsActionable = true;
                objApprovalHeader.Command = command;
                //  UpdateRequestOnEmail(command, PersonnelNo, objApprovalHeader);
                // ViewBag.Message = "Approval ID:" + App_ID + " Processed Successfully.";
                EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(Convert.ToInt32(App_ID));
                await Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

            }
            else
            {
                objApprovalHeader.IsActionable = false;
                ViewBag.Message = "Invalid action.";
            }
            return View(objApprovalHeader);
        }

        [AllowAnonymous]
        [HttpPost]
        public void RejectOnMailF_Confirm(ApprovalHeader objApprovalHeader)
        {
            objApprovalHeader.ApproverAction = 2;
            UpdateRequestOnEmail(objApprovalHeader.Command, objApprovalHeader.APP_PendingWith, objApprovalHeader);
        }


        private void UpdateRequestOnEmail(string command, string PersonnelNo, ApprovalHeader objApprovalHeader)
        {
            objApprovalHeader.Approver = PersonnelNo;
            Result output = null;
            try
            {
                objApprovalHeader.Approver = PersonnelNo;
                if (command == "Review") // this is for Reviewer
                {

                    output = ApprovalRepo.ReviewApproval(objApprovalHeader);
                }
                else if (command == "Submit") // this is for approver
                {
                    output = UpdateRequestStatus(objApprovalHeader);
                }
                if (output.Sucessful == 1)
                {
                    //EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(objApprovalHeader.APP_ID);
                    //Utility.SendMailThroughSendGrid(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);

                }
                else
                {
                    Utility.LogExceptions(output);

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;

            }
        }

        private Result UpdateRequestStatus(ApprovalHeader objApprovalHeader)
        {
            if (string.IsNullOrEmpty(objApprovalHeader.Approver))
            {
                var loginEmp1 = (Users)Session["loginEmp"];
                string approvalBy = loginEmp1.US_PersonnelNo;
                objApprovalHeader.Approver = approvalBy;
            }


            ApproverAction ObjApproverAction = new ApproverAction();
            ObjApproverAction.AppID = Convert.ToInt32(objApprovalHeader.APP_ID);
            ObjApproverAction.Action = Convert.ToInt32(objApprovalHeader.ApproverAction);
            ObjApproverAction.Approver = objApprovalHeader.Approver;
            ObjApproverAction.Remarks = objApprovalHeader.ApproverRemarks;
            ObjApproverAction.SendBackTo = objApprovalHeader.SendBackForRework;
            ObjApproverAction.SelectedaFinalEmailUsers_Entry = objApprovalHeader.SelectedaFinalEmailUsers_Entry;

            Result rs = ApprovalRepo.UpdateRequestStatus(ObjApproverAction);   //db.OPO_UpdateRequestStatus(ReqNo, approvalBy, action, remark);
            //var requestApproval = db.Vw_OPO_ReqApprovalByManager.Where(item => item.ApprovalBy == loginEmp1.PersonnelNo && item.LevelStatus == "P").ToList().Count();
            if (rs.Sucessful == 1)
            {
                if (objApprovalHeader.ApproverAction == 1) // approve
                {
                    // SendEmail(new MyDele(Email.Approver_Approved), ReqNo);
                    //Email email = factsheetService.GetFactsheetMail(objApprovalHeader.HeaderData.Factsheet_No);
                    //SendMail(email);
                }
                else if (objApprovalHeader.ApproverAction == 2) // reject
                {
                    //SendEmail(new MyDele(Email.Approver_Reject), ReqNo);
                }
                else if (objApprovalHeader.ApproverAction == 3) // send Back
                {
                    //SendEmail(new MyDele(Email.approver_sentback), ReqNo);
                    // Email email = factsheetService.GetFactsheetMail(objApprovalHeader.HeaderData.Factsheet_No);
                    //  SendMail(email);
                }
                //  Session["PendingReq"] = requestApproval;
                TempData["DisplayMsg"] = "Approval  ID: " + objApprovalHeader.APP_ID + " has been processed.";
                return rs;
            }
            else
            {
                TempData["DisplayMsg"] = rs.ReturnVal;
                return rs;
            }
        }

        private void GetMasterViewBags()
        {
            var loginEmp1 = (Users)Session["loginEmp"];

            if (loginEmp1 == null)
                RedirectToAction("Login", "Account");
            List<SelectListItem> listApprovalTrail = ApprovalRepo.GetDropdownList("APP_MST_ApprovalTrail");
            ViewBag.ApprovalTrail = listApprovalTrail;

            List<SelectListItem> listFinalEmailList = ApprovalRepo.GetDropdownList("APP_MST_FinalEmailList_Users");
            ViewBag.FinalEmailList = listFinalEmailList;

            List<SelectListItem> listFiscalYear = ApprovalRepo.GetDropdownList("APP_MST_FiscalYear");
            ViewBag.FiscalYear = listFiscalYear;

            List<SelectListItem> listFunction = ApprovalRepo.GetDropdownList("APP_MST_Function");
            ViewBag.Function = listFunction;

            List<SelectListItem> listPeriod = ApprovalRepo.GetDropdownList("APP_MST_Period");
            ViewBag.Period = listPeriod;

            List<SelectListItem> listTAGNATUREOFTRANSACTION = ApprovalRepo.GetDropdownList("APP_MST_TAGNATUREOFTRANSACTION");
            ViewBag.TAGNATUREOFTRANSACTION = listTAGNATUREOFTRANSACTION;

            List<SelectListItem> listReviewers = ApprovalRepo.GetDropdownList("APP_VW_USERS");
            listReviewers = listReviewers.Where(x => x.Value != loginEmp1.US_PersonnelNo).ToList();
            ViewBag.Reviewers = listReviewers;


        }
        public ActionResult GetApprovalTrailUsers(int AT_ID, int AT_Value=0 )
        {
            var approvalTrailUserList = ApprovalRepo.GetApprovalTrailUserList(AT_ID, AT_Value).Where(x => x.ATU_IsActive == true && x.ATU_ThresholdValue <= AT_Value).ToList();
            return PartialView("_vwApprovalTrailUsersList", approvalTrailUserList);
        }
        public ActionResult GetFinalEmailUsers(int EL_ID)
        {
            var finalEmailUsersList = ApprovalRepo.GetFinalEmailUserList();
            return PartialView("_vwFinalEmailUsersList", finalEmailUsersList);
        }

        public IList<Files> GetUploadedFiles()
        {
            var loginEmp = (Users)Session["loginEmp"];
            //int i = 0;
            IList<Files> UploadedFiles = new List<Files>();
            int NoOfFiles = Request.Files.Count;

            for (int i = 0; i < NoOfFiles; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file.FileName != "")
                {
                    Files myFile = new Files();
                    // myFile.FileData = FileToByteArray(Request.Files[i]);
                    myFile.FileName = file.FileName;
                    myFile.FileType = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
                    myFile.UploadedBy = loginEmp.US_PersonnelNo;
                    myFile.UploadedDate = System.DateTime.Now.ToString();

                    UploadedFiles.Add(myFile);
                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/Documents"), fileName);
                    //file.SaveAs(path);
                    // i++;

                }
            }
            return UploadedFiles;
        }
        public byte[] FileToByteArray(HttpPostedFileBase file)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                file.SaveAs(Server.MapPath("~/Documents/") + file.FileName);
                byte[] data = System.IO.File.ReadAllBytes(Server.MapPath("~/Documents/") + file.FileName);
                // return ms.ToArray();
                FileInfo fl = new FileInfo(Server.MapPath("~/Documents/") + file.FileName);
                fl.Delete();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult DeleteDoument(int FileID)
        {
            IList<Files> lstFile = new List<Files>();
            if (Session["UploadedFiles"] != null)
            {
                lstFile = (IList<Files>)Session["UploadedFiles"];
            }
            if (lstFile.Count > 0)
            {
                Files file = lstFile.Where(x => x.FileID == FileID).FirstOrDefault();
                if (file != null)
                {
                    lstFile.Remove(file);

                    Result rs = ApprovalRepo.DeleteDocument(FileID);
                    if (rs.exception == null)
                    {
                        //delete Physical File also.
                        //  string path = Server.MapPath("~/Attachments/" + file.FileName);
                        //string path = Request.MapPath("~/Attachments/" + file.FileName);
                        string path = HostingEnvironment.MapPath("~/Attachments/" + file.FileName);
                        // FileInfo fileToDelete = new FileInfo(path);
                        //if (fileToDelete.Exists)//check file exsit or not  
                        if (System.IO.File.Exists(path))
                        {
                            try
                            {
                                //fileToDelete.Delete();

                                System.IO.File.Delete(path);
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }

                            TempData["DisplayMsg"] = "File:" + file.FileName + " Delete Successfully!";
                        }
                    }
                }
            }

            ViewBag.IsActionable = true;

            Session["UploadedFiles"] = lstFile;
            return PartialView("_VwUploadedFiles", lstFile);
        }


        public string[] GetDropdownListSelectedFinalEmailUsers(IList<FinalEmailUsersList> lst)
        {
            int ln = lst.Count();
            string[] arrStr = new string[ln];

            if (lst != null && lst.Count > 0)
            {
                int i = 0;
                foreach (var item in lst)
                {
                    arrStr[i] = item.ELU_Email;
                    i++;
                }

            }
            return arrStr;
        }

        public ActionResult DownloadDoument(string Filename)
        {

            if (Path.GetExtension(Filename) == ".pdf")
            {
                Response.ContentType = "application/pdf";
                //Response.AppendHeader("Content-Disposition", "attachment; filename="+Filename+".pdf");
                Response.AddHeader("content-disposition", "inline;filename=" + Filename);
                Response.TransmitFile(Server.MapPath("~/Attachments/" + Filename));
                Response.End();
                return null;
            }
            else
            {
                string fullPath = Path.Combine(Server.MapPath("~/Attachments/"), Filename);
                //return File(fullPath);
                byte[] fileBytes = GetFile(fullPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Filename);
            }
        }
        public ActionResult DownloadSupportDoument(string Filename)
        {

            if (Path.GetExtension(Filename) == ".pdf")
            {
                Response.ContentType = "application/pdf";
                //Response.AppendHeader("Content-Disposition", "attachment; filename="+Filename+".pdf");
                Response.AddHeader("content-disposition", "inline;filename=" + Filename);
                Response.TransmitFile(Server.MapPath("~/Attachments/ReferenceDocuments/" + Filename));
                Response.End();
                return null;
            }
            else
            {
                string fullPath = Path.Combine(Server.MapPath("~/Attachments/ReferenceDocuments/"), Filename);
                //return File(fullPath);
                byte[] fileBytes = GetFile(fullPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Filename);
            }
        }
        public byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        public ActionResult TestPage()
        {
            return View("TestPage");
        }

        [HttpPost]
        public async Task<ActionResult> TestPage(string ApprovalID)
        {
            EmailMessage EmailToSend = ApprovalRepo.GetEmailDetails(Convert.ToInt32(ApprovalID));
            await Utility.SendMailThroughSendGrid_Async(EmailToSend.To, EmailToSend.CC, EmailToSend.Bcc, EmailToSend.EmailSubject, EmailToSend.EmailBody, EmailToSend.Attachments);
            return View("TestPage");
        }
    }
}