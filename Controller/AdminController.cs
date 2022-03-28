using ApprovalPortal.Models;
using ApprovalPortal.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApprovalPortal.Controllers
{
    [APActionFilterAdmin]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private readonly IApprovalRepo ApprovalRepo;
        public AdminController(IApprovalRepo _approvalRepo)
        {
            this.ApprovalRepo = _approvalRepo;
        }

        public ActionResult IndexFiscalYear()
        {
            IList<Masters> lstFiscalYears = ApprovalRepo.GetMasters("FISCALYEAR");
            Session["FISCALYEAR"] = lstFiscalYears;
            return View(lstFiscalYears);
        }

        public ActionResult AddEditFiscalYear(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "FISCALYEAR";
            master = GetMasterData(ID, master);
            return View(master);
        }

        [HttpPost]
        public ActionResult AddEditFiscalYear(Masters master)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            master.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            master.MasterName = "FISCALYEAR";
            Result output = ApprovalRepo.AddEditMaster(master);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexFiscalYear", "Admin");

        }

        // Period
        public ActionResult IndexPeriod()
        {
            IList<Masters> lstPeriod = ApprovalRepo.GetMasters("PERIOD");
            Session["PERIOD"] = lstPeriod;
            return View(lstPeriod);
        }

        public ActionResult AddEditPeriod(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "PERIOD";
            master = GetMasterData(ID, master);
            return View(master);
        }

        private Masters GetMasterData(int? ID, Masters master)
        {
            IList<Masters> lstPeriods;

            master.IsActive = true;

            if (ID != null)
            {

                if (Session[master.MasterName] != null)
                {
                    lstPeriods = (IList<Masters>)Session[master.MasterName];
                }
                else
                {
                    lstPeriods = ApprovalRepo.GetMasters(master.MasterName);
                }
                master = lstPeriods.Where(x => x.ID == Convert.ToInt32(ID)).FirstOrDefault();
            }
            return master;
        }

        [HttpPost]
        public ActionResult AddEditPeriod(Masters master)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            master.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            master.MasterName = "PERIOD";
            Result output = ApprovalRepo.AddEditMaster(master);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexPeriod", "Admin");

        }
        //

        //Function
        public ActionResult IndexFunction()
        {
            IList<Masters> lstData = ApprovalRepo.GetMasters("FUNCTION");
            Session["FUNCTION"] = lstData;
            return View(lstData);
        }

        public ActionResult AddEditFunction(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "FUNCTION";
            master = GetMasterData(ID, master);
            return View(master);
        }

        [HttpPost]
        public ActionResult AddEditFunction(Masters master)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            master.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            master.MasterName = "FUNCTION";
            Result output = ApprovalRepo.AddEditMaster(master);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexFunction", "Admin");

        }

        // Function


        //TNT
        //Function
        public ActionResult IndexTNT()
        {
            IList<Masters> lstData = ApprovalRepo.GetMasters("TNT");
            Session["TNT"] = lstData;
            return View(lstData);
        }

        public ActionResult AddEditTNT(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "TNT";
            master = GetMasterData(ID, master);
            return View(master);
        }

        [HttpPost]
        public ActionResult AddEditTNT(Masters master)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            master.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            master.MasterName = "TNT";
            Result output = ApprovalRepo.AddEditMaster(master);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexTNT", "Admin");

        }
        // ENd TNT


        ///Start of FinalEmailUsers
        public ActionResult IndexFinalEmailUsers()
        {
            IList<Masters> lstData = ApprovalRepo.GetMasters("FINALEMAILLIST");
            Session["TNT"] = lstData;
            return View(lstData);
        }

        public ActionResult AddEditFinalEmailUsers(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "TNT";
            master = GetMasterData(ID, master);
            return View(master);
        }

        [HttpPost]
        public ActionResult AddEditFinalEmailUsers(Masters master)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            master.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            master.MasterName = "FINALEMAILLIST";
            Result output = ApprovalRepo.AddEditMaster(master);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexFinalEmailUsers", "Admin");

        }

        // end of Final Email Users


        //Start of Approval Trail
        public ActionResult IndexApprovalTrail()
        {
            IList<Masters> lstData = ApprovalRepo.GetMasters("APPROVALTRAIL");
            Session["APPROVALTRAIL"] = lstData;
            return View(lstData);
        }

        public ActionResult Downloadexcel_AllApprovalTrailsWithUsers()
        {
            if (Session["loginEmp"] != null)
            {
                DataTable tblChannelData = ApprovalRepo.GetApprovalTrailDetailList();

                // RemoveNotRequiredColumns(tblChannelData);

                string contentType = MimeMapping.GetMimeMapping("xlsx");

                string fileName = "attachment;filename=\"Approval Trail User Detail List.xlsx\"";

                ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                wbook.Worksheets.Add(tblChannelData, "tab1");
                // Prepare the response
                var httpResponse = this.HttpContext.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Provide you file name here
                // httpResponse.AddHeader("content-disposition", "attachment;filename=\"+Samplefile+.xlsx\"");
                httpResponse.AddHeader("content-disposition", fileName);

                // Flush the workbook to the Response.OutputStream
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger exlog = new ExceptionLogger(ex.Message, "Approver-DownloadExcel", ex.StackTrace, System.DateTime.Now, Convert.ToString(Session["PersonnelNo"]));
                }
                // Response.AppendHeader("Content-Disposition", cd.ToString());
                // return File(filedata, contentType);
                httpResponse.End();
            }
            return null;
        }

        private void RemoveNotRequiredColumns(DataTable tbl)
        {
            RemoveColumns(tbl, "APP_FiscalYearId");
            RemoveColumns(tbl, "APP_PeriodId");
            RemoveColumns(tbl, "APP_FunctionId");
            RemoveColumns(tbl, "App_IsSentBack");
            RemoveColumns(tbl, "App_SentBackTo");
            RemoveColumns(tbl, "MyAction");
            RemoveColumns(tbl, "APP_StatusId");
            RemoveColumns(tbl, "APP_ELId");
            RemoveColumns(tbl, "APP_TAGNatureId");
            RemoveColumns(tbl, "APP_ApprovalTrailId");
            RemoveColumns(tbl, "APP_Description");
            RemoveColumns(tbl, "APP_Creator");
            RemoveColumns(tbl, "APP_ReviewerID");
            RemoveColumns(tbl, "APP_PendingWith");
        }
        private static void RemoveColumns(DataTable tbl, string columnName)
        {
            if (tbl.Columns.IndexOf(columnName) >= 0)
            {
                tbl.Columns.Remove(columnName);
            }
        }
        public ActionResult DownloadExcel_AllApprovals()
        {
            if (Session["loginEmp"] != null)
            {
                Users user = (Users)Session["loginEmp"];

                DataTable tblAllApprovalData = ApprovalRepo.GetApprovalList_DataSet(null, user.US_PersonnelNo, Convert.ToInt32(user.US_Admin)).Tables[0];

                RemoveNotRequiredColumns(tblAllApprovalData);

                string contentType = MimeMapping.GetMimeMapping("xlsx");

                string fileName = "attachment;filename=\"All Approvals List.xlsx\"";

                ClosedXML.Excel.XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook();
                wbook.Worksheets.Add(tblAllApprovalData, "tab1");
                // Prepare the response
                var httpResponse = this.HttpContext.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Provide you file name here
                // httpResponse.AddHeader("content-disposition", "attachment;filename=\"+Samplefile+.xlsx\"");
                httpResponse.AddHeader("content-disposition", fileName);

                // Flush the workbook to the Response.OutputStream
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger exlog = new ExceptionLogger(ex.Message, "Approver-DownloadExcel", ex.StackTrace, System.DateTime.Now, Convert.ToString(Session["PersonnelNo"]));
                }
                // Response.AppendHeader("Content-Disposition", cd.ToString());
                // return File(filedata, contentType);
                httpResponse.End();
            }
            return null;
        }

        public ActionResult AddEditApprovalTrail(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "APPROVALTRAIL";
            master = GetMasterData(ID, master);
            return View(master);
        }

        [HttpPost]
        public ActionResult AddEditApprovalTrail(Masters master)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            master.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            master.MasterName = "APPROVALTRAIL";
            Result output = ApprovalRepo.AddEditMaster(master);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexApprovalTrail", "Admin");

        }

        // end of Approval trail

        //Start of Approval Trail usER
        public ActionResult IndexApprovalTrailUsers(int? AT_ID, int? ATU_ID)
        {
            ApprovalTrailUsersList trailUser = new ApprovalTrailUsersList();
            IList<Masters> lstApprovalTrailData = (IList<Masters>)Session["APPROVALTRAIL"];
            string approvalTrailName = lstApprovalTrailData.Where(x => x.ID == AT_ID).Select(x => x.Text).FirstOrDefault();

            IEnumerable<ApprovalTrailUsersList> lstData = ApprovalRepo.GetApprovalTrailUserList(Convert.ToInt32(AT_ID));
            trailUser.AT_ID = Convert.ToInt32(AT_ID);
            trailUser.TrailUserList = lstData;
            trailUser.ApprovalTrailName = approvalTrailName;
            if (ATU_ID != null)
            {
                var user = lstData.Where(x => x.ATU_ID == ATU_ID).FirstOrDefault();

                trailUser.ATU_ID = user.ATU_ID;
                trailUser.ATU_IsActive = user.ATU_IsActive;
                trailUser.ATU_PersonnelNo = user.ATU_PersonnelNo;
                trailUser.ATU_ApprovingLevel = user.ATU_ApprovingLevel;
                trailUser.ATU_ThresholdValue = user.ATU_ThresholdValue;
            }
            GetMasterViewBags();
            return View(trailUser);
        }

        public ActionResult AddEditApprovalTrailUsers(int? ID)
        {
            Masters master = new Masters();
            master.MasterName = "APPROVALTRAIL";
            master = GetMasterData(ID, master);
            return View(master);
        }

        [HttpPost]
        public ActionResult AddEditApprovalTrailUsers(ApprovalTrailUsersList approvalTrailUser)
        {

            var loginEmp1 = (Users)Session["loginEmp"];
            approvalTrailUser.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;

            Result output = ApprovalRepo.AddEditApprovalTrailUsers(approvalTrailUser);
            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                return View("Error", output);
            }
            return RedirectToAction("IndexApprovalTrailUsers", new { AT_ID = approvalTrailUser.AT_ID });

        }



        //Start of manage user
        public ActionResult IndexUsers()
        {
            IList<Users> lstData = ApprovalRepo.GetUsers();
            Session["Users"] = lstData;
            return View(lstData);
        }
        public ActionResult AddEditUsers(int? US_ID)
        {
            GetMasterViewBags();
            IList<Users> lstData = null;
            if (Session["Users"] != null)
            {
                lstData = (List<Users>)Session["Users"];
            }
            else
            {
                lstData = ApprovalRepo.GetUsers();
            }
            var user = lstData.Where(x => x.US_ID == US_ID).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        public ActionResult AddEditUsers(Users user)
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            user.CreatedByPersonnelNo = loginEmp1.US_PersonnelNo;
            Result output = ApprovalRepo.AddEditUser(user);

            if (output.Sucessful == 1)
            {
                string strDisplayMsg = output.ReturnVal + " Proccesed successfully.";
                TempData["DisplayMsg"] = strDisplayMsg;
            }
            else
            {
                TempData["ErrorMsg"] = output.ShortMsg;
                // return View("Error", output);
            }
            return RedirectToAction("IndexUsers", "Admin");
        }
        // end of Users

        private void GetMasterViewBags()
        {
            List<SelectListItem> listAllCMSUsers = ApprovalRepo.GetDropdownList("ALLCMSUSERS");
            ViewBag.CMSUsers = listAllCMSUsers;

            List<SelectListItem> listAllApprovers = ApprovalRepo.GetDropdownList("APP_VW_APPROVERS");
            ViewBag.AllApprovers = listAllApprovers;


        }

        // POST: /Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AllApprovalsList()
        {
            var loginEmp1 = (Users)Session["loginEmp"];
            //ViewBag.ManagerData = loginEmp1.ManagerName;
            var allGridData = ApprovalRepo.GetApprovalList(null, loginEmp1.US_PersonnelNo, 1).OrderByDescending(app => app.APP_ID).ToList();

            ApprovalList allRequestList = new ApprovalList();
            allRequestList.AllRequests = allGridData;
            allRequestList.AllRequests.ToList().ForEach(c => c.ActionMethod = "ShowApproval");
            allRequestList.AllRequests.ToList().ForEach(c => c.IsActionable = false);
            return View("AllApprovalsList", allRequestList.AllRequests);
        }
        public ActionResult ShowApproval(int app_id, bool IsActionable)
        {
            if (Session["loginEmp"] == null)
            {
                TempData["DisplayMsg"] = "Session Timeout, Please login again.";
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("ShowApproval", "Approval", new { @app_id = app_id, @IsActionable = IsActionable });

        }
    }
}
