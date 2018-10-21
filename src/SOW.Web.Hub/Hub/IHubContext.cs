/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    public interface IHubContext {
        string ConnectionId { get; set; }
        ClientProxy Client { get; set; }
        string UserName { get; set; }
        bool IsAdmin { get; set; }
        string HubName { get; set; }
        string Hash { get; set; }
        string Time { get; set; }
    }
}
