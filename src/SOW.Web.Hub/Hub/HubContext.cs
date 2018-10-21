/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using System;
    public class HubContext : IHubContext {
        public HubContext( ) { }
        public string ConnectionId { get; set; }
        public ClientProxy Client { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public string HubName { get; set; }
        public string Hash { get; set; }
        public string Time { get; set; }
    }
}
