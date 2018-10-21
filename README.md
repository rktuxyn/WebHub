# WebHub C#
<b>DotNet 4.6.1</b>

<b>1) Map the route for the web socket Hub </b>
```c#
using Owin;
using Microsoft.Owin;
using System.Collections.Generic;
[assembly: OwinStartup( typeof( SOW.WebApi.View.Startup ) )]

namespace SOW.WebApi.View {
    using SOW.Web.Hub.Core.Extensions;
    using SOW.Web.Hub.Core.Hub;
    public class Startup {
        public void Configuration( IAppBuilder app ) {
            app.MapWebSocketRoute<Manager>( "/hub", new HubConfiguration {
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true,
                EnableCrossDomain = true,
                CrossDomains = new List<string> { "sockethub.pc", "api.pc", "lms.pc" },
                StandardHubName = true
            } );
        }
    }
}
```
<b>2) Inherit Hub from SOW.Web.Hub.Core.Hubs </b>
```c#
namespace SOW.Web.Hub.View {
    using System;
    using System.Threading.Tasks;
    using SOW.Web.Hub.Core;
    public class Manager : Hubs {
        public override Task OnConnected( ) {
            ( ( dynamic )Clients.AllExcept( base.ConnectionId ) ).onNewUserConnected( base.Hash, base.ConnectionId, base.UserName, DateTime.Now.ToString( ) );
            return base.OnConnected( );
        }
        public override Task OnReconnected( ) {
            ( ( dynamic )Clients.AllExcept( base.ConnectionId ) ).onReconnected( base.Hash, base.ConnectionId, base.UserName, DateTime.Now.ToString( ) );
            return base.OnReconnected( );
        }
        public override Task OnDisconnected( bool stopCalled ) {
            ( ( dynamic )Clients.AllExcept( base.ConnectionId ) ).onDisconnectUser( base.Hash, base.UserName );
            return base.OnDisconnected( stopCalled );
        }
         #region Private Message
        [Authorize( Roles = "Team, Admin" )]
        public Task SendPrivateMessage( string toHash, string message ) {
            return ( Task )( ( dynamic )Clients.ClientHash( toHash ) ).onPrivateMessage( Hash, UserName, message );
        }
        #endregion  Private Message
    }
}
```

## Screenshots
<img alt="Repository View" src="https://github.com/RKTUXYN/WebHub/blob/master/src/SOW.Web.Hub/WebHub.jpg" width="600"/>&nbsp;

<p> THANKS to Bryce Godfrey (https://github.com/bryceg/) for Owin.WebSocket</p>
