using ApprovalPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApprovalPortal.Repository
{
    public class ApprovalRepo : IApprovalRepo
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString.ToString();
        //private static string connectionString16 = ConfigurationManager.ConnectionStrings["connectionstring16"].ConnectionString.ToString();
        DatabaseHelper db = new DatabaseHelper();
        CultureInfo cultures = new CultureInfo("en-US");

        public IEnumerable<Models.ApprovalHeader> GetApprovalList(int? app_Id,string PersonnelNo, int IsAdmin)
        {

            //connectionString = ConfigurationManager.ConnectionStrings["pmsnewconnectionstring"].ConnectionString.ToString();
            //DataSet ds = getDataFromDB("select * from TVM_Vw_ProgramHeaderList", CommandType.Text);

            //DataSet ds = getDataFromDB("TVM_SP_GetProgramHeaderList", CommandType.StoredProcedure);
           // db.AddParameter("@APP_ID", app_Id);
            db.AddParameter("@PersonnelNo", PersonnelNo);
            db.AddParameter("@IsAdmin", IsAdmin);
            DataSet ds = db.ExecuteDataSet("APP_GetApprovalList", CommandType.StoredProcedure);

            IList<ApprovalHeader> programHeaderList = new List<ApprovalHeader>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    programHeaderList = (from DataRow dr in ds.Tables[0].Rows
                                         select new ApprovalHeader()
                                         {
                                             APP_ID = Convert.ToInt32(dr["APP_ID"]),
                                             APP_ApprovalFor = Convert.ToString(dr["APP_ApprovalFor"]),
                                             APP_Description = Convert.ToString(dr["APP_Description"]),
                                             APP_FiscalYearId = Convert.ToInt32(dr["APP_FiscalYearId"]),
                                             APP_PeriodId = Convert.ToInt32(dr["APP_PeriodId"]),
                                             APP_FunctionId = Convert.ToInt32(dr["APP_FunctionId"]),
                                             APP_TAGNatureId = Convert.ToInt32(dr["APP_TAGNatureId"]),
                                             APP_Creator = Convert.ToString(dr["APP_Creator"]),
                                             APP_CreatorRemarks = Convert.ToString(dr["APP_CreatorRemarks"]),
                                             APP_ReviewerID = Convert.ToString(dr["APP_ReviewerID"]),

                                             APP_ReviewerRemarks = Convert.ToString(dr["APP_ReviewerRemarks"]),
                                             APP_ApprovalTrailId = Convert.ToInt32(dr["APP_ApprovalTrailId"]),
                                             // APP_ELId = Convert.ToInt32(dr["APP_ELId"]),
                                             APP_StatusId = Convert.ToInt32(dr["APP_StatusId"]),
                                             APP_PendingWith = Convert.ToString(dr["APP_PendingWith"]),
                                             APP_CreatedOn = Convert.ToDateTime(dr["APP_CreatedOn"]),
                                             PeriodText = Convert.ToString(dr["PeriodText"]),
                                             FiscalYearText = Convert.ToString(dr["FiscalYearText"]),
                                             FunctionText = Convert.ToString(dr["FunctionText"]),
                                             TNTText = Convert.ToString(dr["TNTText"]),
                                             ReviewerName = Convert.ToString(dr["ReviewerName"]),
                                             CreatorName = Convert.ToString(dr["CreatorName"]),
                                             ApprovalTrailName = Convert.ToString(dr["ApprovalTrailName"]),
                                             //EL_EmailListName = Convert.ToString(dr["EL_EmailListName"]),
                                             PendingWith = Convert.ToString(dr["PendingWith"]),
                                             ApprovalStatus = Convert.ToString(dr["ApprovalStatus"]),
                                             IsSentBack = Convert.ToBoolean(dr["App_IsSentBack"]),
                                             SentBackToUser = Convert.ToString(dr["App_SentBackTo"]),
                                             App_Value = Convert.ToDecimal(dr["App_Value"]),
                                              MyAction = Convert.ToString(dr["MyAction"]),
                                         }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return programHeaderList;
        }

        public DataSet GetApprovalList_DataSet(int? app_Id, string PersonnelNo, int IsAdmin)
        {
            db.AddParameter("@PersonnelNo", PersonnelNo);
            db.AddParameter("@IsAdmin", IsAdmin);
            DataSet ds = db.ExecuteDataSet("[APP_GetApprovalList_WithApproverRemarks]", CommandType.StoredProcedure);
            return ds;
        }
        public ApprovalHeader GetApprovalAllData(int app_Id)
        {
            db.AddParameter("@APP_ID", app_Id);
          
            DataSet ds = db.ExecuteDataSet("APP_GetApprovalAllData", CommandType.StoredProcedure);

            IList<ApprovalHeader> programHeaderList = new List<ApprovalHeader>();
            ApprovalHeader appHeader = new ApprovalHeader();
            IList<ApprovalMatrix> approvalMatrixList = new List<ApprovalMatrix>();
            IList<ApprovalTransaction> approvalTransactionList = new List<ApprovalTransaction>();
            IList<Files> attachmentList = new List<Files>();
            IList<ApprovalNotes> approvalNotesList = new List<ApprovalNotes>();
            IList<FinalEmailUsersList> SelectedFinalEmailUsers = new List<FinalEmailUsersList>();

            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    programHeaderList = (from DataRow dr in ds.Tables[0].Rows
                                         select new ApprovalHeader()
                                         {
                                             APP_ID = Convert.ToInt32(dr["APP_ID"]),
                                             APP_ApprovalFor = Convert.ToString(dr["APP_ApprovalFor"]),
                                             APP_Description = Convert.ToString(dr["APP_Description"]),
                                             APP_FiscalYearId = Convert.ToInt32(dr["APP_FiscalYearId"]),
                                             APP_PeriodId = Convert.ToInt32(dr["APP_PeriodId"]),
                                             APP_FunctionId = Convert.ToInt32(dr["APP_FunctionId"]),
                                             APP_TAGNatureId = Convert.ToInt32(dr["APP_TAGNatureId"]),
                                             APP_Creator = Convert.ToString(dr["APP_Creator"]),
                                             APP_CreatorRemarks = Convert.ToString(dr["APP_CreatorRemarks"]),
                                             APP_ReviewerID = Convert.ToString(dr["APP_ReviewerID"]),

                                             APP_ReviewerRemarks = Convert.ToString(dr["APP_ReviewerRemarks"]),
                                             APP_ApprovalTrailId = Convert.ToInt32(dr["APP_ApprovalTrailId"]),
                                             // APP_ELId = Convert.ToInt32(dr["APP_ELId"]),
                                             APP_StatusId = Convert.ToInt32(dr["APP_StatusId"]),
                                             APP_PendingWith = Convert.ToString(dr["APP_PendingWith"]),
                                             APP_CreatedOn = Convert.ToDateTime(dr["APP_CreatedOn"]),
                                             PeriodText = Convert.ToString(dr["PeriodText"]),
                                             FiscalYearText = Convert.ToString(dr["FiscalYearText"]),
                                             FunctionText = Convert.ToString(dr["FunctionText"]),
                                             TNTText = Convert.ToString(dr["TNTText"]),
                                             ReviewerName = Convert.ToString(dr["ReviewerName"]),
                                             CreatorName = Convert.ToString(dr["CreatorName"]),
                                             ApprovalTrailName = Convert.ToString(dr["ApprovalTrailName"]),
                                             EL_EmailListName = Convert.ToString(dr["EL_EmailListName"]),
                                             PendingWith = Convert.ToString(dr["PendingWith"]),
                                             ApprovalStatus = Convert.ToString(dr["ApprovalStatus"]),
                                             IsSentBack = Convert.ToBoolean(dr["App_IsSentBack"]),
                                             SentBackToUser = Convert.ToString(dr["App_SentBackTo"]),
                                             App_Value = Convert.ToDecimal(dr["App_Value"])
                                         }
                        ).ToList();

                    appHeader = programHeaderList.FirstOrDefault();
                    //Table 1 for Approval MAtrix
                    approvalMatrixList = (from DataRow dr in ds.Tables[1].Rows
                                          select new ApprovalMatrix()
                                          {
                                              AM_APP_ID = Convert.ToInt32(dr["AM_APP_ID"]),
                                              AM_ID = Convert.ToInt32(dr["AM_ID"]),
                                              AM_ApprovingLevel = Convert.ToInt32(dr["AM_ApprovingLevel"]),
                                              AM_EmployeeName = Convert.ToString(dr["AM_EmployeeName"]),
                                              AM_PersonnelNo = Convert.ToString(dr["AM_PersonnelNo"]),
                                              AM_Role = Convert.ToString(dr["AM_Role"]),

                                          }
                       ).ToList();

                    //Table 2 for Approval Transaction
                    approvalTransactionList = (from DataRow dr in ds.Tables[2].Rows
                                               select new ApprovalTransaction()
                                               {
                                                   AT_APP_ID = Convert.ToInt32(dr["AT_APP_ID"]),
                                                   AT_ID = Convert.ToInt32(dr["AT_ID"]),
                                                   //AT_ApprovingLevel = Convert.ToInt32(dr["AT_ApprovingLevel"]),
                                                   AT_EmployeeName = Convert.ToString(dr["AT_EmployeeName"]),
                                                   AT_PersonnelNo = Convert.ToString(dr["AT_PersonnelNo"]),
                                                   AT_Role = Convert.ToString(dr["AT_Role"]),
                                                   AT_ApprovingDate = Convert.ToString(dr["AT_ApprovingDate"]),
                                                   AT_ApprovingRemarks = Convert.ToString(dr["AT_Remarks"]),
                                                   AT_StatusId = Convert.ToInt32(dr["AT_Status"]),
                                                   ApproverAction = Convert.ToString(dr["ApproverAction"]),

                                               }
                       ).ToList();
                    //Table 3 for Approval Files
                    attachmentList = (from DataRow dr in ds.Tables[3].Rows
                                      select new Files()
                                      {
                                          APP_ID = Convert.ToInt32(dr["APP_ID"]),
                                          EmployeeName = Convert.ToString(dr["EmployeeName"]),
                                          FileName = Convert.ToString(dr["FileName"]),
                                          FileID = Convert.ToInt32(dr["FileID"]),
                                          FileType = Convert.ToString(dr["FileType"]),
                                          UploadedBy = Convert.ToString(dr["UploadedBy"]),
                                          //UploadedDate = Convert.ToDateTime(dr["UploadedDate"])
                                          UploadedDate = Convert.ToString(dr["UploadedDate"])

                                      }
                      ).ToList();

                    //Table 4 for Approval Notes
                    approvalNotesList = (from DataRow dr in ds.Tables[4].Rows
                                         select new ApprovalNotes()
                                         {
                                             AN_ID = Convert.ToInt32(dr["AN_ID"]),
                                             AN_AddedOn = Convert.ToDateTime(dr["AN_AddedOn"]),
                                             AN_APP_ID = Convert.ToInt32(dr["AN_APP_ID"]),
                                             AN_Notes = Convert.ToString(dr["AN_Notes"]),
                                             AN_PersonnelNo = Convert.ToString(dr["AN_PersonnelNo"]),
                                             EmployeeName = Convert.ToString(dr["EmployeeName"]),

                                         }
                    ).ToList();


                    SelectedFinalEmailUsers = (from DataRow dr in ds.Tables[5].Rows
                                               select new FinalEmailUsersList()
                                               {
                                                   //  ELU_ID = Convert.ToInt32(dr["ELU_ID"]),
                                                   ELU_Email = Convert.ToString(dr["FE_Email"]),
                                               }
                   ).ToList();

                    appHeader = programHeaderList.FirstOrDefault();
                    appHeader.ApprovalMatrixList = approvalMatrixList;
                    appHeader.ApprovalTransactionList = approvalTransactionList;
                    appHeader.UploadedAttachmentsList = attachmentList;
                    appHeader.ApprovalNotesList = approvalNotesList;
                    appHeader.SelectedFinalEmailUsers = SelectedFinalEmailUsers;
                }



            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return appHeader;
        }


        public List<SelectListItem> GetDropdownList(string masterTableName)
        {
            string Qry = string.Empty;
            switch (masterTableName.ToUpper())
            {
                case "APP_MST_FISCALYEAR":
                    Qry = "select FY_ID, FY_Text from APP_MST_FiscalYear where FY_IsActive=1";
                    break;
                case "APP_MST_APPROVALTRAIL":
                    Qry = "select AT_ID, AT_TrailName from APP_MST_ApprovalTrail where AT_IsActive=1  order by 2";
                    break;
                case "APP_MST_FUNCTION":
                    Qry = "select FN_ID, FN_Text from APP_MST_FUNCTION where FN_IsActive=1  order by 2";
                    break;
                case "APP_MST_PERIOD":
                    Qry = "select PD_ID, PD_Text from APP_MST_PERIOD where PD_IsActive=1";
                    break;
                case "APP_MST_TAGNATUREOFTRANSACTION":
                    Qry = "select TNT_ID, TNT_Text from APP_MST_TAGNATUREOFTRANSACTION where TNT_IsActive=1  order by 2";
                    break;
                case "APP_MST_FINALEMAILLIST_USERS":
                    Qry = "select ELU_Email, ELU_Email from APP_MST_FinalEmailList_Users where ELU_IsActive=1  order by 2";
                    break;
                case "APP_VW_USERS":
                    Qry = "select US_PersonnelNo, NamewithSAPID from APP_Vw_Users where US_Reviewer=1 and US_IsActive=1  order by 2";
                    break;
                case "APP_VW_APPROVERS":
                    Qry = "select US_PersonnelNo, NamewithSAPID from APP_Vw_Users where US_Approver=1 and US_IsActive=1";
                    break;
                case "ALLCMSUSERS":
                    Qry = "select PersonnelNo, EmployeeName+' ['+PersonnelNo+']' NameWithSAPID from newcms..sapemployeemaster where Status=3  order by 2";
                    break;


            }
            DataSet ds = getDataFromDB(Qry, CommandType.Text);
            List<SelectListItem> listdropdown = new List<SelectListItem>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    listdropdown.Add(new SelectListItem { Text = Convert.ToString(dr[1]), Value = Convert.ToString(dr[0]), });
                }

            }
            return listdropdown;
        }
        private static DataSet getDataFromDB(string SQLQuery, CommandType commandType)
        {

            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = commandType;
                        if (con.State == System.Data.ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        sda.Fill(ds);
                        con.Close();
                    }
                }
            }

            return ds;
        }

        private void LogException(Exception ex)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TVM_LogException", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExceptionMessage", ex.Message);
                    cmd.Parameters.AddWithValue("@ExceptionStack", ex.StackTrace);
                    cmd.Parameters.AddWithValue("@InnerException", ex.InnerException);
                    cmd.Parameters.AddWithValue("@OccurredBy", "");
                    con.Open();
                    var rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
        }
        private static Result ExecNonQry(SqlConnection con, SqlCommand cmd)
        {
            Result rs = new Result();
            try
            {
                int noOfRowUpdated = cmd.ExecuteNonQuery();
                rs.ReturnVal = Convert.ToString(noOfRowUpdated);
                rs.RowsEffected = noOfRowUpdated;
                rs.Sucessful = 1;
                con.Close();
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0;
                rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace;
                rs.ShortMsg = ex.Message;
                con.Close();
            }
            return rs;
        }


        public Result SaveApproval(ApprovalHeader appHeader)
        {
            SqlParameter prmFinalEmailUsers = new SqlParameter();
            prmFinalEmailUsers.ParameterName = "@FinalEmailUsers";
            prmFinalEmailUsers.SqlDbType = SqlDbType.Structured;
            prmFinalEmailUsers.Value = ConvertListToDataTable(appHeader.SelectedaFinalEmailUsers_Entry);
            prmFinalEmailUsers.TypeName = "[dbo].[App_type_FinalEmail_Txn]";

            Result rs = new Result();
            var prmAPP_ID = new SqlParameter("@APP_ID", SqlDbType.Int);
            prmAPP_ID.Direction = ParameterDirection.InputOutput;
            prmAPP_ID.Value = appHeader.APP_ID;

            DataTable dtFiles = getDataTableFiles(appHeader.Attachments.ToList());

            var prmUploadFiles = new SqlParameter("@UploadedFiles", SqlDbType.Structured);
            prmUploadFiles.Value = dtFiles;
            prmUploadFiles.TypeName = "dbo.APP_TYPE_FILES";


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("APP_AddEditApprovalHeader", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(prmAPP_ID);
                    cmd.Parameters.AddWithValue("@APP_ApprovalFor", appHeader.APP_ApprovalFor);
                    cmd.Parameters.AddWithValue("@APP_Description", appHeader.APP_Description);
                    cmd.Parameters.AddWithValue("@APP_FiscalYearId", appHeader.APP_FiscalYearId);
                    cmd.Parameters.AddWithValue("@APP_PeriodId", appHeader.APP_PeriodId);
                    cmd.Parameters.AddWithValue("@APP_FunctionId", appHeader.APP_FunctionId); 
                    cmd.Parameters.AddWithValue("@APP_TAGNatureId", appHeader.APP_TAGNatureId);
                    cmd.Parameters.AddWithValue("@APP_Creator", appHeader.APP_Creator);
                    cmd.Parameters.AddWithValue("@APP_CreatorRemarks", appHeader.APP_CreatorRemarks);
                    cmd.Parameters.AddWithValue("@APP_ReviewerID", appHeader.APP_ReviewerID);
                    cmd.Parameters.AddWithValue("@APP_ReviewerRemarks", appHeader.APP_ReviewerRemarks);
                    cmd.Parameters.AddWithValue("@APP_ApprovalTrailId", appHeader.APP_ApprovalTrailId);
                    cmd.Parameters.AddWithValue("@APP_Value", appHeader.App_Value);
                    cmd.Parameters.AddWithValue("@APP_StatusId", appHeader.APP_StatusId);
                    // cmd.Parameters.AddWithValue("@FinalEmailUsers", ConvertListToDataTable(appHeader.SelectedaFinalEmailUsers_Entry));
                    cmd.Parameters.Add(prmFinalEmailUsers);
                    cmd.Parameters.Add(prmUploadFiles);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(prmAPP_ID.Value);

                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public Result ReviewApproval(ApprovalHeader appHeader)
        {
            SqlParameter prmFinalEmailUsers = new SqlParameter();
            prmFinalEmailUsers.ParameterName = "@FinalEmailUsers";
            prmFinalEmailUsers.SqlDbType = SqlDbType.Structured;
            prmFinalEmailUsers.Value = ConvertListToDataTable(appHeader.SelectedaFinalEmailUsers_Entry);
            prmFinalEmailUsers.TypeName = "[dbo].[App_type_FinalEmail_Txn]";

            Result rs = new Result();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("APP_ReviewerAction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@APP_ID", appHeader.APP_ID);
                    //cmd.Parameters.AddWithValue("@APP_ApprovalTrailId", appHeader.APP_ApprovalTrailId);
                    // cmd.Parameters.AddWithValue("@APP_ELId", appHeader.APP_ELId);

                    cmd.Parameters.AddWithValue("@APP_ReviewerRemarks", appHeader.ApproverRemarks);
                    cmd.Parameters.AddWithValue("@APP_ApproverAction", appHeader.ApproverAction);
                    cmd.Parameters.AddWithValue("@App_Approver", appHeader.Approver);
                    cmd.Parameters.AddWithValue("@SendBackToUser", appHeader.SendBackForRework);
                    //cmd.Parameters.AddWithValue("@FinalEmailUsers", ConvertListToDataTable(appHeader.SelectedaFinalEmailUsers_Entry));
                    cmd.Parameters.Add(prmFinalEmailUsers);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(appHeader.APP_ID);
                rs.RowsEffected = 1;
                rs.Sucessful = 1;
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;

        }

        private DataTable getDataTableFiles(List<Files> UploadedFiles)
        {
            DataTable dtFiles = new DataTable();
            dtFiles.Columns.Add("FileID", typeof(Int32));
            dtFiles.Columns.Add("FileName", typeof(string));
            //  dtFiles.Columns.Add("FileData", System.Type.GetType("System.Byte[]"));
            dtFiles.Columns.Add("FileType", typeof(string));
            dtFiles.Columns.Add("APP_ID", typeof(Int32));
            dtFiles.Columns.Add("UploadedBy", typeof(string));
            dtFiles.Columns.Add("UploadedDate", typeof(DateTime));

            foreach (Files file in UploadedFiles)
            {
                DataRow dr = dtFiles.NewRow();
                dr["FileID"] = 0;// file.FileID;
                dr["FileName"] = file.FileName;//.Replace("'","''");
                // dr["FileData"] = file.FileData;
                dr["FileType"] = file.FileType;
                dr["APP_ID"] = 0;// file.FactsheetID;
                dr["UploadedBy"] = file.UploadedBy;
                dr["UploadedDate"] = file.UploadedDate;

                dtFiles.Rows.Add(dr);

            }
            return dtFiles;
        }


        public int ValidateUserEssel(string personnelNo, string password)
        {
            var personnelNoParameter = personnelNo != null ?
               new ObjectParameter("PersonnelNo", personnelNo) :
               new ObjectParameter("PersonnelNo", typeof(string));

            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));

            var isChangeParameter = new ObjectParameter("IsChange", 0);
               

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ValidateUserEssel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PersonnelNo", Convert.ToString(personnelNoParameter.Value));
                    cmd.Parameters.AddWithValue("@password", Convert.ToString(passwordParameter.Value));
                    cmd.Parameters.AddWithValue("@isValid", 0);
                    cmd.Parameters.AddWithValue("@isAdmin", 0);
                    cmd.Parameters.AddWithValue("@IsChange", Convert.ToInt32(isChangeParameter.Value));
                    cmd.Parameters.AddWithValue("@isExpiry", 0);
                    con.Open();
                    object rs = cmd.ExecuteScalar();

                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();

                    int IsValiduser = Convert.ToInt32(rs);
                    return IsValiduser;
                }
            }
        }

        public Users GetUserInfo(string personnelNo)
        {
            IList<Users> lstUsers = new List<Users>();
            Users user = new Users();
            string qry = "select * from APP_Vw_Users where US_PersonnelNo='" + personnelNo + "' and US_IsActive=1";
            DataSet ds = db.ExecuteDataSet(qry, CommandType.Text);
            try
            {
                lstUsers = DataSetToUsersMapping(lstUsers, ds);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            if (lstUsers.Count > 0)
                user = lstUsers[0];
            return user;
        }

        public IList<Users> GetUsers()
        {
            IList<Users> lstUsers = new List<Users>();
            Users user = new Users();
            string qry = "select * from APP_Vw_Users";
            DataSet ds = db.ExecuteDataSet(qry, CommandType.Text);
            try
            {
                lstUsers = DataSetToUsersMapping(lstUsers, ds);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return lstUsers;
        }
        public DataTable GetApprovalTrailDetailList()
        {
            DataTable dt = new DataTable();
            string qry = "select * from App_Vw_ApprovalTrailsDetailedList Order by [Approval Trail Name] , Approver_Level, [Approver_Threshold Value] Asc ";
            DataSet ds = db.ExecuteDataSet(qry, CommandType.Text);
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return dt;
        }

        public IList<Users> GetAllCMSUsers()
        {
            IList<Users> lstUsers = new List<Users>();
            Users user = new Users();
            string qry = "select PersonnelNo, EmployeeName+' ['+PersonnelNo+']' NameWithSAPID from newcms..sapemployeemaster where Status=3";
            DataSet ds = db.ExecuteDataSet(qry, CommandType.Text);
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    lstUsers = (from DataRow dr in ds.Tables[0].Rows
                                select new Users()
                                {
                                    US_PersonnelNo = Convert.ToString(dr["PersonnelNo"]),
                                    NameWithSAPID = Convert.ToString(dr["NameWithSAPID"]),
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return lstUsers;
        }

        private static IList<Users> DataSetToUsersMapping(IList<Users> lstUsers, DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0)
            {
                lstUsers = (from DataRow dr in ds.Tables[0].Rows
                            select new Users()
                            {
                                US_ID = Convert.ToInt32(dr["US_ID"]),
                                US_Admin = Convert.ToBoolean(dr["US_Admin"]),
                                US_IsActive = Convert.ToBoolean(dr["US_IsActive"]),
                                US_Creator = Convert.ToBoolean(dr["US_Creator"]),
                                US_Reviewer = Convert.ToBoolean(dr["US_Reviewer"]),
                                US_PersonnelNo = Convert.ToString(dr["US_PersonnelNo"]),
                                EmailWithSAPID = Convert.ToString(dr["EmailWithSAPID"]),
                                NameWithSAPID = Convert.ToString(dr["NameWithSAPID"]),
                                US_Approver = Convert.ToBoolean(dr["US_Approver"]),
                                CreatedOn=Convert.ToString(dr["CreatedOn"]),
                                UpdatedOn=Convert.ToString(dr["UpdatedOn"]),
                                CreatedByName=Convert.ToString(dr["CreatedByName"]),
                                UpdatedByName=Convert.ToString(dr["UpdatedByName"]),
                            }).ToList();
            }
            return lstUsers;
        }

        public void LogError(ExceptionLogger logger)
        {
            //throw new NotImplementedException();
        }


        public Result UpdateRequestStatus(ApproverAction ObjApproverAction)
        {

            Result rs = new Result();
            SqlParameter prmFinalEmailUsers = new SqlParameter();
            prmFinalEmailUsers.ParameterName = "@FinalEmailUsers";
            prmFinalEmailUsers.SqlDbType = SqlDbType.Structured;
            prmFinalEmailUsers.TypeName = "[dbo].[App_type_FinalEmail_Txn]";
            if (ObjApproverAction.SelectedaFinalEmailUsers_Entry != null)
            prmFinalEmailUsers.Value = ConvertListToDataTable(ObjApproverAction.SelectedaFinalEmailUsers_Entry);


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("APP_UpdateRequestStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@APP_ID", ObjApproverAction.AppID);
                    cmd.Parameters.AddWithValue("@Approver", ObjApproverAction.Approver);
                    cmd.Parameters.AddWithValue("@SendBackToUser", ObjApproverAction.SendBackTo);
                    cmd.Parameters.AddWithValue("@Action", ObjApproverAction.Action);
                    cmd.Parameters.AddWithValue("@Remarks", ObjApproverAction.Remarks);
                    // cmd.Parameters.AddWithValue("@APP_ELId", ObjApproverAction.APP_ELId);
                    //cmd.Parameters.AddWithValue("@FinalEmailUsers", ConvertListToDataTable(ObjApproverAction.SelectedaFinalEmailUsers_Entry));
                    if (prmFinalEmailUsers.Value!=null)
                    cmd.Parameters.Add(prmFinalEmailUsers);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(ObjApproverAction.AppID);
                rs.RowsEffected = 1;
                rs.Sucessful = 1;
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public Result UpdateRequestStatusFromCreator(ApproverAction ObjApproverAction)
        {

            Result rs = new Result();
            SqlParameter prmFinalEmailUsers = new SqlParameter();
            prmFinalEmailUsers.ParameterName = "@FinalEmailUsers";
            prmFinalEmailUsers.SqlDbType = SqlDbType.Structured;
            prmFinalEmailUsers.TypeName = "[dbo].[App_type_FinalEmail_Txn]";
            if (ObjApproverAction.SelectedaFinalEmailUsers_Entry != null)
                prmFinalEmailUsers.Value = ConvertListToDataTable(ObjApproverAction.SelectedaFinalEmailUsers_Entry);


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("APP_UpdateRequestStatusFromCreator", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@APP_ID", ObjApproverAction.AppID);
                    cmd.Parameters.AddWithValue("@Approver", ObjApproverAction.Approver);
                    cmd.Parameters.AddWithValue("@Action", ObjApproverAction.Action);
                    cmd.Parameters.AddWithValue("@Remarks", ObjApproverAction.Remarks);
                    
                    if (prmFinalEmailUsers.Value != null)
                        cmd.Parameters.Add(prmFinalEmailUsers);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(ObjApproverAction.AppID);
                rs.RowsEffected = 1;
                rs.Sucessful = 1;
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public IEnumerable<Models.ApprovalNotes> GetNotesList(int? app_Id, int? AN_ID)
        {
            db.AddParameter("@AN_APP_ID", app_Id);
            db.AddParameter("@AN_ID", AN_ID);
            DataSet ds = db.ExecuteDataSet("App_GetNotes", CommandType.StoredProcedure);

            IList<ApprovalNotes> ApprovalNotesList = new List<ApprovalNotes>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    ApprovalNotesList = (from DataRow dr in ds.Tables[0].Rows
                                         select new ApprovalNotes()
                                         {
                                             AN_APP_ID = Convert.ToInt32(dr["AN_APP_ID"]),
                                             AN_Notes = Convert.ToString(dr["AN_Notes"]),
                                             AN_ID = Convert.ToInt32(dr["AN_ID"]),
                                             AN_PersonnelNo = Convert.ToString(dr["AN_PersonnelNo"]),
                                             EmployeeName = Convert.ToString(dr["EmployeeName"]),
                                             AN_AddedOn = Convert.ToDateTime(dr["AN_AddedOn"])
                                         }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw ex;
            }

            return ApprovalNotesList;
        }

        public Result SaveNote(ApprovalNotes note)
        {
            Result rs = new Result();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_AddNotes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AN_ID", note.AN_ID);
                    cmd.Parameters.AddWithValue("@AN_APP_ID", note.AN_APP_ID);
                    cmd.Parameters.AddWithValue("@AN_PersonnelNo", note.AN_PersonnelNo);
                    cmd.Parameters.AddWithValue("@AN_Notes", note.AN_Notes);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(rs.RowsEffected);
                rs.RowsEffected = 1;
                rs.Sucessful = 1;
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public Result DeleteNote(int AN_ID)
        {
            Result rs = new Result();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_DeleteNotes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AN_ID", AN_ID);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(rs.RowsEffected);
                rs.RowsEffected = 1;
                rs.Sucessful = 1;
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public IEnumerable<Models.FinalEmailUsersList> GetFinalEmailUserList()
        {
            // db.AddParameter("@EL_ID", EL_ID);
            DataSet ds = db.ExecuteDataSet("App_FinalEmailUserList", CommandType.StoredProcedure);

            IList<FinalEmailUsersList> finalEmailUsersList = new List<FinalEmailUsersList>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    finalEmailUsersList = (from DataRow dr in ds.Tables[0].Rows
                                           select new FinalEmailUsersList()
                                         {
                                             ELU_ID = Convert.ToInt32(dr["ELU_ID"]),
                                             ELU_IsActive = Convert.ToBoolean(dr["ELU_IsActive"]),
                                             ELU_Email = Convert.ToString(dr["ELU_Email"]),
                                             // EmployeeName = Convert.ToString(dr["EmployeeName"]),
                                             //  EMail = Convert.ToString(dr["EMail"]),
                                         }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return finalEmailUsersList;
        }

        public IEnumerable<Models.ApprovalTrailUsersList> GetApprovalTrailUserList(int AT_ID, int AT_Value=0)
        {
            db.AddParameter("@AT_ID", AT_ID);
            db.AddParameter("@AT_Value", AT_Value);
            DataSet ds = db.ExecuteDataSet("App_GetApprovalTrailUserList", CommandType.StoredProcedure);

            IList<ApprovalTrailUsersList> approvalTrailUsersList = new List<ApprovalTrailUsersList>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    approvalTrailUsersList = (from DataRow dr in ds.Tables[0].Rows
                                              select new ApprovalTrailUsersList()
                                            {
                                                AT_ID = Convert.ToInt32(dr["AT_ID"]),
                                                ATU_ID = Convert.ToInt32(dr["ATU_ID"]),
                                                ATU_ApprovingLevel = Convert.ToInt32(dr["ATU_ApprovingLevel"]),
                                                ATU_IsActive = Convert.ToBoolean(dr["ATU_IsActive"]),
                                                EmployeeName = Convert.ToString(dr["EmployeeName"]),
                                                ATU_PersonnelNo = Convert.ToString(dr["ATU_PersonnelNo"]),
                                                CreatedByName = Convert.ToString(dr["CreatedByName"]),
                                                UpdatedByName = Convert.ToString(dr["UpdatedByName"]),
                                                CreatedOn = Convert.ToString(dr["CreatedOn"]),
                                                UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                                                ApprovalTrailName = Convert.ToString(dr["ApprovalTrailName"]),
                                                ATU_ThresholdValue=Convert.ToDecimal(dr["ATU_ThresholdValue"])
                                            }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return approvalTrailUsersList;
        }

        public DataTable ConvertListToDataTable(string[] finalEmailUsersList)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Email");
            dt.Columns.Add(dc);
            if (finalEmailUsersList != null)
            {
                if (finalEmailUsersList.Count() != 0)
                {
                    foreach (var item in finalEmailUsersList)
                    {
                        DataRow row = dt.NewRow();
                        row[dc.ColumnName] = item;
                        dt.Rows.Add(row);
                    }
                }
            }
            return dt;
        }


        public IList<FiscalYear> GetFiscalYears()
        {
            DataSet ds = db.ExecuteDataSet("select * from App_Vw_FiscalYear", CommandType.Text);

            IList<FiscalYear> FiscalYearList = new List<FiscalYear>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    FiscalYearList = (from DataRow dr in ds.Tables[0].Rows
                                      select new FiscalYear()
                                         {
                                             ID = Convert.ToInt32(dr["FY_ID"]),
                                             CreatedByName = Convert.ToString(dr["FY_CreatedBy"]),
                                             Text = Convert.ToString(dr["FY_Text"]),
                                             IsActive = Convert.ToBoolean(dr["FY_IsActive"]),
                                             CreatedByPersonnelNo = Convert.ToString(dr["CreatedBy"]),
                                             UpdatedByName = Convert.ToString(dr["UpdatedBy"]),
                                             CreatedOn = Convert.ToString(dr["FY_CreatedOn"]),
                                             UpdatedOn = Convert.ToString(dr["FY_UpdatedOn"]),
                                             UpdatedByPersonnelNo = Convert.ToString(dr["FY_UpdatedBy"]),
                                         }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return FiscalYearList;
        }

        public Result AddEditFiscalYear(FiscalYear fiscalYear)
        {
            //App_AddEditFiscalYear
            Result rs = new Result();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_AddEditFiscalYear", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FY_ID", fiscalYear.ID);
                    cmd.Parameters.AddWithValue("@FY_Text", fiscalYear.Text);
                    cmd.Parameters.AddWithValue("@FY_IsActive", fiscalYear.IsActive);
                    cmd.Parameters.AddWithValue("@FY_User", fiscalYear.CreatedByPersonnelNo);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString("Fiscal Year:" + fiscalYear.Text);

                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public IList<Period> GetPeriods()
        {
            DataSet ds = db.ExecuteDataSet("select * from App_Vw_Period", CommandType.Text);

            IList<Period> PeriodList = new List<Period>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    PeriodList = (from DataRow dr in ds.Tables[0].Rows
                                  select new Period()
                                  {
                                      ID = Convert.ToInt32(dr["PD_ID"]),
                                      CreatedByPersonnelNo = Convert.ToString(dr["PD_CreatedBy"]),
                                      Text = Convert.ToString(dr["PD_Text"]),
                                      IsActive = Convert.ToBoolean(dr["PD_IsActive"]),
                                      CreatedByName = Convert.ToString(dr["CreatedBy"]),
                                      UpdatedByName = Convert.ToString(dr["UpdatedBy"]),
                                      CreatedOn = Convert.ToString(dr["PD_CreatedOn"]),
                                      UpdatedOn = Convert.ToString(dr["PD_UpdatedOn"]),
                                      UpdatedByPersonnelNo = Convert.ToString(dr["PD_UpdatedBy"]),
                                  }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return PeriodList;
        }

        public Result AddEditPeriod(Period period)
        {
            //App_AddEditFiscalYear
            Result rs = new Result();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_AddEditPeriod", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PD_ID", period.ID);
                    cmd.Parameters.AddWithValue("@PD_Text", period.Text);
                    cmd.Parameters.AddWithValue("@PD_IsActive", period.IsActive);
                    cmd.Parameters.AddWithValue("@PD_User", period.CreatedByPersonnelNo);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString("Period:" + period.Text);

                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public IList<Masters> GetMasters(string masterType)
        {
            string qry = "";
            switch (masterType.ToUpper())
            {
                case "FISCALYEAR":
                    qry = "select * from App_Vw_FiscalYear";
                    break;
                case "PERIOD":
                    qry = "select * from App_Vw_Period";
                    break;
                case "FUNCTION":
                    qry = "select * from App_Vw_FUnction";
                    break;
                case "TNT":
                    qry = "select * from app_vw_TnT";
                    break;

                case "FINALEMAILLIST":
                    qry = "select * from app_vw_FinalEmailUsers";
                    break;

                case "APPROVALTRAIL":
                    qry = "select * from app_vw_ApprovalTrail";
                    break;

            }
            DataSet ds = db.ExecuteDataSet(qry, CommandType.Text);

            IList<Masters> MastersList = new List<Masters>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    MastersList = (from DataRow dr in ds.Tables[0].Rows
                                   select new Masters()
                                   {
                                       ID = Convert.ToInt32(dr["ID"]),
                                       CreatedByName = Convert.ToString(dr["CreatedByName"]),
                                       Text = Convert.ToString(dr["Text"]),
                                       IsActive = Convert.ToBoolean(dr["IsActive"]),
                                       CreatedByPersonnelNo = Convert.ToString(dr["CreatedByPersonnelNo"]),
                                       UpdatedByName = Convert.ToString(dr["UpdatedByName"]),
                                       CreatedOn = Convert.ToString(dr["CreatedOn"]),
                                       UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                                       UpdatedByPersonnelNo = Convert.ToString(dr["UpdatedByPersonnelNo"]),
                                   }
                        ).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return MastersList;
        }

        public Result AddEditMaster(Masters master)
        {
            //App_AddEditFiscalYear
            Result rs = new Result();
            string qry = "";
            switch (master.MasterName.ToUpper())
            {
                case "FISCALYEAR":
                    qry = "App_AddEditFiscalYear";
                    break;
                case "PERIOD":
                    qry = "App_AddEditPeriod";
                    break;
                case "FUNCTION":
                    qry = "App_AddEditFunction";
                    break;
                case "TNT":
                    qry = "App_AddEditTNT";
                    break;
                case "FINALEMAILLIST":
                    qry = "App_AddEditFinalEmailList";
                    break;
                case "APPROVALTRAIL":
                    qry = "App_AddEditApprovalTrail";
                    break;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(qry, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", master.ID);
                    cmd.Parameters.AddWithValue("@Text", master.Text);
                    cmd.Parameters.AddWithValue("@IsActive", master.IsActive);
                    cmd.Parameters.AddWithValue("@User", master.CreatedByPersonnelNo);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString(master.MasterName + " " + master.Text);

                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public Result AddEditUser(Users user)
        {
            Result rs = new Result();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_AddEditUsers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@US_ID", user.US_ID);
                    cmd.Parameters.AddWithValue("@US_PersonnelNo", user.US_PersonnelNo);
                    cmd.Parameters.AddWithValue("@US_Creator", user.US_Creator);
                    cmd.Parameters.AddWithValue("@US_Reviewer", user.US_Reviewer);
                    cmd.Parameters.AddWithValue("@US_Approver", user.US_Approver);
                    cmd.Parameters.AddWithValue("@US_Admin", user.US_Admin);
                    cmd.Parameters.AddWithValue("@US_IsActive", user.US_IsActive);
                    cmd.Parameters.AddWithValue("@US_CreatedBy", user.CreatedByPersonnelNo);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {
                if (rs.ReturnVal == "-1")
                {
                    rs.exception = new Exception("Duplicate Entry");
                }
                else
                {
                    rs.ReturnVal = Convert.ToString("User: " + user.NameWithSAPID);
                }
                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public Result AddEditApprovalTrailUsers(ApprovalTrailUsersList approvalTrailUser)
        {
            Result rs = new Result();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_AddEditApprovalTrailUsers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ATU_ID", approvalTrailUser.ATU_ID);
                    cmd.Parameters.AddWithValue("@AT_ID", approvalTrailUser.AT_ID);
                    cmd.Parameters.AddWithValue("@ATU_PersonnelNo", approvalTrailUser.ATU_PersonnelNo);
                    cmd.Parameters.AddWithValue("@ATU_ApprovingLevel", approvalTrailUser.ATU_ApprovingLevel);
                    cmd.Parameters.AddWithValue("@ATU_ThresholdValue", approvalTrailUser.ATU_ThresholdValue);
                    cmd.Parameters.AddWithValue("@ATU_IsActive", approvalTrailUser.ATU_IsActive);
                    cmd.Parameters.AddWithValue("@ATU_CreatedBy", approvalTrailUser.CreatedByPersonnelNo);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString("User: " + approvalTrailUser.EmployeeName);

                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }

        public Result DeleteDocument(int FileID)
        {
            Result rs = new Result();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("App_DeleteDocument", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FileID", FileID);
                    con.Open();
                    rs = ExecNonQry(con, cmd);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    //return rs;
                }
            }
            string strDisplayMsg = string.Empty;
            try
            {

                rs.ReturnVal = Convert.ToString("File: " + FileID);

                if (rs.exception != null)
                {
                    rs.RowsEffected = 0;
                    rs.ShortMsg = rs.exception.Message;
                    rs.Sucessful = 0;
                }
                else
                {
                    rs.RowsEffected = 1;
                    rs.Sucessful = 1;
                }
            }
            catch (Exception ex)
            {
                rs.Sucessful = 0; rs.exception = ex;
                rs.RowsEffected = 0;
                rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
            }
            return rs;
        }


        public EmailMessage GetEmailDetails(int App_Id)
        {
            db.AddParameter("@APP_ID", App_Id);
            DataSet ds = db.ExecuteDataSet("App_GetApprovalEmail_New", CommandType.StoredProcedure);

            IList<EmailMessage> EmailList = new List<EmailMessage>();
            EmailMessage myEmailToSend = new EmailMessage();
            IList<Files> attachedFiles = new List<Files>();
            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    EmailList = (from DataRow dr in ds.Tables[0].Rows
                                    select new EmailMessage()
                                         {
                                             To = Convert.ToString(dr["To"]),
                                             Bcc = Convert.ToString(dr["Bcc"]),
                                             CC = Convert.ToString(dr["CC"]),
                                             EmailSubject = Convert.ToString(dr["EmailSubject"]),
                                             EmailBody = Convert.ToString(dr["EmailBody"]),
                                         }
                        ).ToList();
                }
                if (ds != null && ds.Tables.Count > 1)
                {
                    attachedFiles = (from DataRow dr in ds.Tables[1].Rows
                                 select new Files()
                                 {
                                     FileName = Convert.ToString(dr["FileName"]),
                                 }
                        ).ToList();
                }

                myEmailToSend = EmailList.FirstOrDefault();
                myEmailToSend.Attachments = attachedFiles;
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw ex;
            }

            return myEmailToSend;
        }

    }
}

