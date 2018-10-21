/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using Microsoft.Owin;
    using System.Threading.Tasks;
    public interface IHubs {
        IOwinContext Context { get; }
        IHubCallerContext<dynamic> Clients { get; set; }
        Task OnConnected( );
        Task OnDisconnected( bool stopCalled );
        Task OnReconnected( );
    }
}
