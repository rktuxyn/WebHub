/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using System;

namespace SOW.Web.Hub.Core {
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false )]
    public sealed class RouteAttribute : Attribute {
        readonly string _route;

        public RouteAttribute( string route ) {
            this._route = route;
        }

        public string Route {
            get { return _route; }
        }
    }
}
