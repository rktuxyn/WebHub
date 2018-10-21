/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using System.Collections.Generic;
    public interface IHubConnectionContext<T> {
        IList<IHubContext> GetAdmins( params string[] hubname );
        IList<IHubContext> GetAdmins(  );
        T Admin( string hubname );
        ClientProxy ProxyAdmin( string hubname );
        T Admin( params string[] userName );
        T User( string hubname );
        T User( params string[] userName );
        ClientProxy ProxyUser( params string[] userName );
        IList<IHubContext> GetActiveUserContext( string exceptUser, string hubname );
        IList<IHubContext> GetActiveUserContext( string exceptUser );

        IList<IUserInfo> GetActiveUser( string exceptUser, string hubname );
        IList<IUserInfo> GetActiveHash( string exceptHash, string hubname );
        IList<IHubContext> GetUserByHash( string hash, string hubname );
        IList<IHubContext> GetUser( string userName, string hubname );
        IList<IHubContext> GetUserByConnectionId( string connectionId, string hubname );

        IList<IUserInfo> GetActiveUser( params string[] exceptUser );
        IList<IUserInfo> GetActiveHash( params string[] exceptHash );
        IList<IHubContext> GetUserByHash( params string[] hash );
        IList<IHubContext> GetUser( params string[] userName );
        IHubContext GetUserContext( string hash );
        IList<IHubContext> GetUserByConnectionId( params string[] connectionId );
        string GetHash( string userName );

        T SendToUserByHash( string hash, string hubname );
        T SendToUserByName( string userName, string hubname );
        T SendToUserByConnectionId(  string connectionId, string hubname );

        T SendToUserByHash( params string[] hash );
        T SendToUserByName( params string[] userName );
        T SendToUserByConnectionId( params string[] connectionId );


        T All( params string[] hubname );
        T All(  );
        T AllExcept(string[] excludeConnectionIds, string[] hubname );
        T AllExcept(params string[] excludeConnectionIds );
        T AllExceptHash( params string[] excludeHashs );

        T AllExceptUser( string[] excludeUsers, string[] hubname );
        T AllExceptUser( params string[] excludeUsers );
        ClientProxy ProxyAllExceptUser( params string[] excludeUsers );
        ClientProxy ProxyAllExceptUser( string[] excludeUsers, string[] hubname );

        T Client( params string[] connectionId );
        T ClientHash( string hash );
        T ClientHash( params string[] hash );
        T Clients( IList<string> connectionIds );

        bool Exists(  string userName, string hubname );
        bool Exists( params string[] userName );
        bool ExistsHash(  string hash, string hubname );
        bool ExistsHash( params string[] hash );
    }
}
