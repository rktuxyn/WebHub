/**
    * Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
    * Copyrights licensed under the New BSD License.
    * See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.View {
    using System;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Security;
    public class Global : System.Web.HttpApplication {
        protected void Application_Start( object sender, EventArgs e ) {
        }
        protected void Application_AuthenticateRequest( Object sender, EventArgs e ) {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if ( authCookie == null ) {
                return;
            }
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt( authCookie.Value );
            string[] roles = authTicket.UserData.Split( new Char[] { ',' } );
            FormsIdentity formsIdentity = new FormsIdentity( authTicket );

            ClaimsIdentity claimsIdentity = new ClaimsIdentity( formsIdentity );

            foreach ( var role in roles ) {
                claimsIdentity.AddClaim(
                    new Claim( ClaimTypes.Role, role ) );
            }
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal( claimsIdentity );
            Context.User = claimsPrincipal;
            return;
        }
    }
}
