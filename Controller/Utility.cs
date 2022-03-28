using ApprovalPortal.Models;
using ApprovalPortal.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EncryptDecryptQuerystring;
using System.Configuration;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ApprovalPortal.Controllers
{
    public class Utility
    {
        static bool mailSent = false;
        public static List<SelectListItem> GetActionMaster()
        {
            List<SelectListItem> listActionMaster = new List<SelectListItem>();
            listActionMaster.Add(new SelectListItem { Text = "Approve", Value = "1", });
            listActionMaster.Add(new SelectListItem { Text = "Reject", Value = "2", });
            listActionMaster.Add(new SelectListItem { Text = "Send for Rework", Value = "3", });
            return listActionMaster;

            /*SSELCONNECTEntities1 db = new ESSELCONNECTEntities1();
            var ActionMasterList = db.OPO_ActionMaster.ToList();
            List<SelectListItem> listActionMaster = new List<SelectListItem>();
            listActionMaster.Add(new SelectListItem { Text = "Select", Value = "0" });
            foreach (OPO_ActionMaster item in ActionMasterList)
            {
                listActionMaster.Add(new SelectListItem { Text = item.AM_Text, Value = item.AM_Value.ToString(), });
            }
            return listActionMaster;*/

        }
        public struct DateTimeSpan
        {
            private readonly int years;
            private readonly int months;
            private readonly int days;
            private readonly int hours;
            private readonly int minutes;
            private readonly int seconds;
            private readonly int milliseconds;
                
            public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
            {
                this.years = years;
                this.months = months;
                this.days = days;
                this.hours = hours;
                this.minutes = minutes;
                this.seconds = seconds;
                this.milliseconds = milliseconds;
            }

            public int Years { get { return years; } }
            public int Months { get { return months; } }
            public int Days { get { return days; } }
            public int Hours { get { return hours; } }
            public int Minutes { get { return minutes; } }
            public int Seconds { get { return seconds; } }
            public int Milliseconds { get { return milliseconds; } }

            enum Phase { Years, Months, Days, Done }

            public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
            {
                if (date2 < date1)
                {
                    var sub = date1;
                    date1 = date2;
                    date2 = sub;
                }

                DateTime current = date1;
                int years = 0;
                int months = 0;
                int days = 0;

                Phase phase = Phase.Years;
                DateTimeSpan span = new DateTimeSpan();
                int officialDay = current.Day;

                while (phase != Phase.Done)
                {
                    switch (phase)
                    {
                        case Phase.Years:
                            if (current.AddYears(years + 1) > date2)
                            {
                                phase = Phase.Months;
                                current = current.AddYears(years);
                            }
                            else
                            {
                                years++;
                            }
                            break;
                        case Phase.Months:
                            if (current.AddMonths(months + 1) > date2)
                            {
                                phase = Phase.Days;
                                current = current.AddMonths(months);
                                if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                    current = current.AddDays(officialDay - current.Day);
                            }
                            else
                            {
                                months++;
                            }
                            break;
                        case Phase.Days:
                            if (current.AddDays(days + 1) > date2)
                            {
                                current = current.AddDays(days);
                                var timespan = date2 - current;
                                span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                                phase = Phase.Done;
                            }
                            else
                            {
                                days++;
                            }
                            break;
                    }
                }

                return span;
            }
        }

        public static SMTPServerInfo objSMTPServerInfo = new SMTPServerInfo();
        static EncryptDecrypt enc = new EncryptDecrypt();

        public static void LogExceptions(Result output)
        {
            ApprovalRepo appRepo = new ApprovalRepo();
            Exception ex = new Exception();
            ex = output.exception;
            ExceptionLogger logger = new ExceptionLogger(ex.Message, "", ex.StackTrace.ToString(), System.DateTime.Now, Convert.ToString(HttpContext.Current.Session["PersonnelNo"]));
            appRepo.LogError(logger);
        }

        public static void SendMail(MailMessage message, IList<Files> AttachedFiles)
        {
            var fromAddress = new MailAddress("Your Gemail Address", "Sender Project System Name");
            const string fromPassword = "Your pass";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            {
                foreach (var aFile in AttachedFiles)
                {
                    var att = new Attachment(HttpContext.Current.Server.MapPath(aFile.FileName));
                    message.Attachments.Add(att);
                }

                smtp.Send(message);
            }
        }

        public static void SendMailThroughSendGrid(string mailTo, string mailCC, string mailBCC, string mailSubject, string mailBody, IList<Files> AttachedFiles)
        {
            string smtpAccount = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SMTPAccount"]);

            objSMTPServerInfo = GetSMTPDetails(smtpAccount);

            string TestingMode = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings["EmailTestingMode"] != null)
            {
                TestingMode = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["EmailTestingMode"]);
            }

            //mailSubject = Regex.Replace(mailSubject, @"\s+", "");

            if (TestingMode.ToUpper() == "DEV")
            {
                mailTo = "bajrang.rathore@zee.com";
                mailCC = "bajrang.rathore@zee.com";

            }

            if (!string.IsNullOrEmpty(mailTo))
            {
                //string from = "billing@zee.esselgroup.com"; //From address   
                string from = objSMTPServerInfo.Email;// "alerts@zee.esselgroup.com"; //From address  
                MailMessage message = new MailMessage();
                message.From = new MailAddress(from);

                string[] arrTo = mailTo.Split(new char[] { ';' });
                foreach (string to in arrTo)
                {
                    if (to.Trim() != "")
                        message.To.Add(to);
                }
                if (mailCC != "")
                {
                    message.CC.Add(mailCC);
                }

                MailAddressCollection mailColl = new MailAddressCollection();

                string[] arrBcc = mailBCC.Split(new char[] { ';' });
                foreach (string bc in arrBcc)
                {
                    if (bc.Trim() != "")
                        message.Bcc.Add(bc);
                }

                // MailAddress bcc = new MailAddress(mailBCC);

                message.Subject = mailSubject;
                message.Body = mailBody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                foreach (var aFile in AttachedFiles)
                {
                    try
                    {
                        var att = new Attachment(HttpContext.Current.Server.MapPath("~/Attachments/" + aFile.FileName));
                        message.Attachments.Add(att);
                    }
                    catch (Exception ex)
                    {

                    }

                }


                SmtpClient client = new SmtpClient();
                client.Host = objSMTPServerInfo.smtpserver;//"smtp.sendgrid.net";
                client.Port = Convert.ToInt32(objSMTPServerInfo.Port);//587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                // client.UseDefaultCredentials = false;
                client.EnableSsl = true;

                client.Timeout = 100000;
                //System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("zee.esselgroup.com", "Abhi@123");//  "P@ssw0rdzee");
                System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential(objSMTPServerInfo.Username, objSMTPServerInfo.Password);//  "P@ssw0rdzee");
                client.EnableSsl = true;
                //client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = basicCredential1;

               // client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                try
                {
                    if (!string.IsNullOrEmpty(mailTo))
                      // client.SendMailAsync(message);              
                      client.Send(message);
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        public static SMTPServerInfo GetSMTPDetails(string name)
        {
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.ConnectionStrings["ConnectionStringForEmail"].ConnectionString);

            string EmailSenderProfile = string.Empty;
            db.AddParameter("@Name", name);
            DataSet dtForEmailSenderProfile = db.ExecuteDataSet("newcms.dbo.CMS_sp_Get_EmailByProfile", CommandType.StoredProcedure);
            if (dtForEmailSenderProfile != null)
            {
                if (dtForEmailSenderProfile.Tables.Count > 0)
                    if (dtForEmailSenderProfile.Tables[0].Rows.Count > 0)
                    {
                        objSMTPServerInfo.Email = Convert.ToString(dtForEmailSenderProfile.Tables[0].Rows[0][0]);
                        objSMTPServerInfo.smtpserver = Convert.ToString(dtForEmailSenderProfile.Tables[0].Rows[0][1]);
                        objSMTPServerInfo.Port = Convert.ToString(dtForEmailSenderProfile.Tables[0].Rows[0][2]);
                        objSMTPServerInfo.Username = Convert.ToString(dtForEmailSenderProfile.Tables[0].Rows[0][3]);
                        objSMTPServerInfo.Password = enc.decryptQueryString(Convert.ToString(dtForEmailSenderProfile.Tables[0].Rows[0][4]));
                    }

            }
            return objSMTPServerInfo;
        }

        public static async Task<bool> SendMailThroughSendGrid_Async(string mailTo, string mailCC, string mailBCC, string mailSubject, string mailBody, IList<Files> AttachedFiles)
        {
            bool isSend = false;

            string smtpAccount = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SMTPAccount"]);

            objSMTPServerInfo = GetSMTPDetails(smtpAccount);

            string TestingMode = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings["EmailTestingMode"] != null)
            {
                TestingMode = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["EmailTestingMode"]);
            }

            //mailSubject = Regex.Replace(mailSubject, @"\s+", "");

            if (TestingMode.ToUpper() == "DEV")
            {
                mailTo = "bajrang.rathore@zee.com";
                mailCC = "bajrang.rathore@zee.com";

            }
            MailMessage message = new MailMessage();

            if (!string.IsNullOrEmpty(mailTo))
            {
                //string from = "billing@zee.esselgroup.com"; //From address   
                string from = objSMTPServerInfo.Email;// "alerts@zee.esselgroup.com"; //From address  
                
                message.From = new MailAddress(from);

                string[] arrTo = mailTo.Split(new char[] { ';' });
                foreach (string to in arrTo)
                {
                    if (to.Trim() != "")
                        message.To.Add(to);
                }
                if (mailCC != "")
                {
                    message.CC.Add(mailCC);
                }

                MailAddressCollection mailColl = new MailAddressCollection();

                string[] arrBcc = mailBCC.Split(new char[] { ';' });
                foreach (string bc in arrBcc)
                {
                    if (bc.Trim() != "")
                        message.Bcc.Add(bc);
                }

                // MailAddress bcc = new MailAddress(mailBCC);

                message.Subject = mailSubject;
                message.Body = mailBody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                foreach (var aFile in AttachedFiles)
                {
                    try
                    {

                       // 
                        //using(Attachment att=new Attachment((HttpContext.Current.Server.MapPath("~/Attachments/" + aFile.FileName))))
                        //{
                        //     message.Attachments.Add(att);
                        //}

                        var att = new Attachment(HttpContext.Current.Server.MapPath("~/Attachments/" + aFile.FileName)); 
                        
                        message.Attachments.Add(att);
                        //att.Dispose();
                    }
                    catch (Exception ex)
                    {

                    }

                }


                SmtpClient client = new SmtpClient();
                client.Host = objSMTPServerInfo.smtpserver;//"smtp.sendgrid.net";
                client.Port = Convert.ToInt32(objSMTPServerInfo.Port);//587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                // client.UseDefaultCredentials = false;
                client.EnableSsl = true;

                client.Timeout = 100000;
                //System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("zee.esselgroup.com", "Abhi@123");//  "P@ssw0rdzee");
                System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential(objSMTPServerInfo.Username, objSMTPServerInfo.Password);//  "P@ssw0rdzee");
                client.EnableSsl = true;
                //client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = basicCredential1;

                //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                try
                {
                    if (!string.IsNullOrEmpty(mailTo))
                        await client.SendMailAsync(message);

                    // Settings.  
                    isSend = true;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    message.Attachments.Dispose();
                }
            }
            message.Attachments.Dispose();
            return isSend;
        }
        //public async Task<bool> SendEmailAsync(string email, string msg, string subject = "")
        //{
        //    // Initialization.  
        //    bool isSend = false;

        //    try
        //    {
        //        // Initialization.  
        //        var body = msg;
        //        var message = new MailMessage();

        //        // Settings.  
        //        message.To.Add(new MailAddress(email));
        //        message.From = new MailAddress(EmailInfo.FROM_EMAIL_ACCOUNT);
        //        message.Subject = !string.IsNullOrEmpty(subject) ? subject : EmailInfo.EMAIL_SUBJECT_DEFAUALT;
        //        message.Body = body;
        //        message.IsBodyHtml = true;

        //        using (var smtp = new SmtpClient())
        //        {
        //            // Settings.  
        //            var credential = new NetworkCredential
        //            {
        //                UserName = EmailInfo.FROM_EMAIL_ACCOUNT,
        //                Password = EmailInfo.FROM_EMAIL_PASSWORD
        //            };

        //            // Settings.  
        //            smtp.Credentials = credential;
        //            smtp.Host = EmailInfo.SMTP_HOST_GMAIL;
        //            smtp.Port = Convert.ToInt32(EmailInfo.SMTP_PORT_GMAIL);
        //            smtp.EnableSsl = true;

        //            // Sending  
        //            await smtp.SendMailAsync(message);

        //            // Settings.  
        //            isSend = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Info  
        //        throw ex;
        //    }

        //    // info.  
        //    return isSend;
        //}  
    }



    public class SMTPServerInfo
    {
        public string Email { get; set; }
        public string smtpserver { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }

}
