/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using Microsoft.Owin;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
    public class AuthorizeAttribute : Attribute, IAuthorizeAttribute {
        public AuthorizeAttribute( ) { }
        public bool RequireOutgoing { get; set; }
        public string Roles { get; set; }
        public string Users { get; set; }
        public virtual bool IsAuthorized( IOwinRequest request ) {
            if ( request.User == null ) return false;
            return request.User.Identity.IsAuthenticated;
        }
        public override bool IsDefaultAttribute( ) {
            if ( string.IsNullOrEmpty( Users ) && string.IsNullOrEmpty( Roles ) )
                return true;
            return false;
        }
        public virtual bool IsInRole( IOwinRequest request ) {
            if ( !IsAuthorized( request ) ) return false;
            if ( string.IsNullOrEmpty( Roles ) ) return true;
            var arr = Roles.Split( ',' );
            int len = arr.Length;
            foreach ( var r in arr ) {
                if ( r == null ) return false;
                var role = r.Trim( );
                if ( request.User.IsInRole( role ) ) {
                    return true;
                }
            }
            return len > 0 ? false : true;
        }
        public virtual bool IsInUsers( IOwinRequest request ) {
            if ( !IsAuthorized( request ) ) return false;
            if ( string.IsNullOrEmpty( Users ) ) return true;
            var userName = request.User.Identity.Name;
            var arr = Users.Split( ',' );
            var resp = arr.FirstOrDefault( a => a == userName );
            return string.IsNullOrEmpty( resp ) ? false : true;
        }
    }
    public static class CustomAttribute {
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>( this ICustomAttributeProvider provider, bool inherit = false )
            where TAttribute : Attribute {
            return provider
                .GetCustomAttributes( typeof( TAttribute ), inherit )
                .Cast<TAttribute>( );
        }
    }
    public interface IAuthorizeAttribute {
        bool RequireOutgoing { get; set; }
        string Users { get; set; }
        string Roles { get; set; }
        bool IsAuthorized( IOwinRequest request );
        bool IsInUsers( IOwinRequest request );
        bool IsInRole( IOwinRequest request );
    }
}
