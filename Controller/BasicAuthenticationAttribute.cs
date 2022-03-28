using System.Linq;
using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Security.Principal;

namespace ApprovalPortal.Controllers
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {


            if (actionContext.Request.Headers.Authorization != null)
            {
                string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                string[] userNamePassword = decodedToken.Split(':');
                string userName = userNamePassword[0];
                string userPassword = userNamePassword[1];


                if (userName == "zeemedia" && userPassword == "zee@123")
                {

                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(userName), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = actionContext.Request
                       .CreateResponse(HttpStatusCode.Unauthorized);
            }

        }
    }
}