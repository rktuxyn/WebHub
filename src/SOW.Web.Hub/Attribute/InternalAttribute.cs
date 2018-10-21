/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using System;

namespace SOW.Web.Hub.Core {
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
    public sealed class InternalAttribute : Attribute {
        readonly string _internal;

        public InternalAttribute( string Internal ) {
            this._internal = Internal;
        }

        public string Internal {
            get { return _internal; }
        }
    }
}
