/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    public interface IHubCallerContext<T> : IHubConnectionContext<T> {
        void Set( IHubContext hubContext );
        void Remove( params string[] excludeConnectionIds );
        void RemoveHash( params string[] excludeHashs );
        void RemoveUser( params string[] excludeUser );
        T Caller { get; }
    }
}
