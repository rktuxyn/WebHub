/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using System.Collections.Generic;
namespace SOW.Web.Hub.Core {
    public interface IHubConfiguration {
        bool EnableJavaScriptProxies { get; set; }
        bool EnableDetailedErrors { get; set; }
        bool EnableCrossDomain { get; set; }
        IList<string> CrossDomains { get; set; }
        bool StandardHubName { get; set; }
        bool AllowInternalRequest { get; set; }
    }
}
