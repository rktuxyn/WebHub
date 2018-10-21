/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using System.Collections.Generic;

namespace SOW.Web.Hub.Core {
    public class HubConfiguration : IHubConfiguration {
        public bool EnableJavaScriptProxies { get; set; }
        public bool EnableDetailedErrors { get; set; }
        public bool EnableCrossDomain { get; set; }
        public IList<string> CrossDomains { get; set; }
        public bool StandardHubName { get; set; }
        public bool AllowInternalRequest { get; set; }
    }
}
