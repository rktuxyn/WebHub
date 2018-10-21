/*
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld).  All rights reserved.
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using Owin;
using Microsoft.Owin;
using System.Collections.Generic;
[assembly: OwinStartup( typeof( SOW.Web.Hub.View.Startup ) )]
namespace SOW.Web.Hub.View {
    using SOW.Web.Hub.Core.Extensions;
    using SOW.Web.Hub.Core;
    public class Startup {
        public void Configuration( IAppBuilder app ) {
            app.MapWebSocketRoute<Manager>( "/hub", new HubConfiguration {
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true,
                EnableCrossDomain = true,
                CrossDomains = new List<string> { "myhub.pc", "mhub.pc" },
                StandardHubName = true
            } );
        }
    }
}