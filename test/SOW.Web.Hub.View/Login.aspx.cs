/*
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld).  All rights reserved.
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.View {
    using System;
    using System.Web;
    using System.Web.Security;
    public partial class Login : System.Web.UI.Page {
        protected void Page_Load( object sender, EventArgs e ) {

        }

        protected void Authenticate_Click( object sender, EventArgs e ) {
            if ( Context.Request.IsAuthenticated ) {
                Response.Redirect( "/", true );
                return;
            }
            string role_id = this.group_type.Value;
            string user = this.chat_user.Value;
            bool error = false;
            if ( string.IsNullOrEmpty( user ) ) {
                this.message.InnerHtml = "User name rquired;"; error = true;
            }
            if ( string.IsNullOrEmpty( role_id ) ) {
                this.message.InnerHtml = "Role name rquired;"; error = true;
            }
            if ( error ) return;
            DateTime expiration = DateTime.Now.AddHours( 24 );
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(/**version*/1,/**userName*/user,
                /**issueDate*/DateTime.Now,/**expiration*/expiration,/**isPersistent*/true,
                /**userData*/role_id,/**cookiePath*/FormsAuthentication.FormsCookiePath
             );
            /** Encrypt the ticket**/
            string hash = FormsAuthentication.Encrypt( authTicket );
            /** Create a new authentication cookie - and set its expiration date**/
            HttpCookie authenticationCookie = new HttpCookie( FormsAuthentication.FormsCookieName, hash ) {
                Shareable = false, HttpOnly = true
            };
            Context.Response.Cookies.Add( authenticationCookie );
            Response.Redirect( "/", true );
        }
    }
}