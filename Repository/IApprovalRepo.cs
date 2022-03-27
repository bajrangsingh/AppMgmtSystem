using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApprovalPortal.Models;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Data;

namespace ApprovalPortal.Repository
{
    public interface IApprovalRepo
    {
        IEnumerable<ApprovalHeader> GetApprovalList(int? app_Id,string PersonnelNo, int IsAdmin);
        List<SelectListItem> GetDropdownList(string masterTableName);

        Result SaveApproval(ApprovalHeader appHeader);
        //Result ValidateUserEssel(string personnelNo, string password, ObjectParameter isValid, ObjectParameter isAdmin, Nullable<int> isChange, ObjectParameter isExpiry);
        int ValidateUserEssel(string personnelNo, string password);
        Users GetUserInfo(string personnelNo);
        void LogError(ExceptionLogger logger);

        Result ReviewApproval(ApprovalHeader appHeader);
        ApprovalHeader GetApprovalAllData(int app_Id);

        Result UpdateRequestStatus(ApproverAction ObjApproverAction);
        IEnumerable<Models.ApprovalNotes> GetNotesList(int? app_Id, int? AN_ID);

        Result SaveNote(ApprovalNotes note);
        Result DeleteNote(int AN_ID);

        IEnumerable<Models.ApprovalTrailUsersList> GetApprovalTrailUserList(int AT_ID, int Value=0);
        IEnumerable<Models.FinalEmailUsersList> GetFinalEmailUserList();



        IList<Masters> GetMasters(string masterType);
        Result AddEditMaster(Masters master);

        IList<Users> GetUsers();

        Result AddEditUser(Users user);
        IList<Users> GetAllCMSUsers();

        Result AddEditApprovalTrailUsers(ApprovalTrailUsersList approvalTrailUser);

        Result DeleteDocument(int FileID);
        EmailMessage GetEmailDetails(int App_Id);

        DataTable GetApprovalTrailDetailList();
        DataSet GetApprovalList_DataSet(int? app_Id, string PersonnelNo, int IsAdmin);

        Result UpdateRequestStatusFromCreator(ApproverAction ObjApproverAction);
    }
}