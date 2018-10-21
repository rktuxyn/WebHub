/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class HubCallerContext<T> : IHubCallerContext<T> {
        static readonly object locker = new object( );
        private static IList<IHubContext> _BaseClient = new List<IHubContext>( );
        private dynamic Caller { get; set; }
        public HubCallerContext( ) { }
        void IHubCallerContext<T>.Set( IHubContext hubContext ) {
            Caller = hubContext.Client;
            _BaseClient.Add( hubContext );
        }
        public bool ExistsHash( string userName, string hubname ) {
            return _BaseClient.Count( a => a.HubName == hubname && a.UserName == userName ) > 0;
        }
        public bool Exists( string hash, string hubname ) {
            return _BaseClient.Count( a => a.HubName == hubname && a.Hash == hash ) > 0;
        }
        public bool Exists( string userName ) {
            return _BaseClient.Count( a =>  a.UserName == userName ) > 0;
        }
        public bool Exists( params string[] userName ) {
            return _BaseClient.Count( a => a.UserName == ( userName.FirstOrDefault( f => f == a.UserName ) ) ) > 0;
        }
        public bool ExistsHash( string hash ) {
            return _BaseClient.Count( a =>a.Hash == hash ) > 0;
        }
        public bool ExistsHash( params string[] hash ) {
            return _BaseClient.Count( a => a.Hash == ( hash.FirstOrDefault( f => f == a.Hash ) ) ) > 0;
        }
        void IHubCallerContext<T>.Remove( params string[] excludeConnectionIds ) {
            excludeConnectionIds.Select( a => {
                var item = _BaseClient.FirstOrDefault( f => f.ConnectionId == a );
                if ( item != null ) {
                    lock ( locker ) {
                        _BaseClient.Remove( item );
                    }
                }
                return a;
            } ).ToList( );
        }
        void IHubCallerContext<T>.RemoveHash( params string[] excludeHashs ) {
            excludeHashs.Select( a => {
                if ( !this.ExistsHash( a ) ) return a;
                _BaseClient.Where( w => w.Hash == a ).Select( s => {
                    lock ( locker ) {
                        _BaseClient.Remove( s );
                    }
                    return s;
                } );
                return a;
            } ).ToList( );
        }
        void IHubCallerContext<T>.RemoveUser( params string[] excludeUser ) {
            excludeUser.Select( a => {
                if ( !this.Exists( a ) ) return a;
                _BaseClient.Where( w => w.UserName == a ).Select( s => {
                    lock ( locker ) {
                        _BaseClient.Remove( s );
                    }
                    return s;
                } );
                return a;
            } ).ToList( );
        }
        public T User( string hubname ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( hubname != a.HubName ) return false;
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T User( params string[] userName ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( userName != null ) {
                        if ( userName.Any( f => f != a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public ClientProxy ProxyUser( params string[] userName ) {
            return new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( userName != null ) {
                        if ( userName.Any( f => f != a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T Admin( string hubname ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( a.IsAdmin != true ) return false;
                    if ( hubname != a.HubName ) return false;
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public ClientProxy ProxyAdmin( string hubname ) {
            return new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( a.IsAdmin != true ) return false;
                    if ( hubname != a.HubName ) return false;
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T Admin( params string[] userName ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( a.IsAdmin != true ) return false;
                    if ( userName != null ) {
                        if ( userName.Any( f => f == a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public IList<IHubContext> GetAdmins( params string[] hubname ) {
            IList<IHubContext> user = new List<IHubContext>( );
            _BaseClient.Where(a=> {
                if ( a.IsAdmin != true ) return false;
                if ( hubname != null ) {
                    return hubname.Any( f => f == a.HubName ) ? true : false;
                }
                return true;
            } ).Select(r=> {
                if ( user.Any( b => b.Hash == r.Hash ) ) return r;
                user.Add( r );
                return r;
            } ).ToList( );
            return user;
        }
        public IList<IHubContext> GetAdmins( ) {
            return _BaseClient.Where(a=> a.IsAdmin == true ).ToList( );
        }
        IList<IHubContext> IHubConnectionContext<T>.GetActiveUserContext( string exceptUser, string hubname ) {
            IList<IHubContext> user = new List<IHubContext>( );
            _BaseClient.Where( w => w.HubName == hubname && w.UserName != exceptUser ).Select( r => {
                if ( user.Any( b => b.Hash == r.Hash ) ) return r;
                user.Add( r );
                return r;
            } ).ToList( );

            return user;
        }
        public IList<IHubContext> GetActiveUserContext( string exceptUser ) {
            IList<IHubContext> user = new List<IHubContext>( );
            _BaseClient.Where( w => w.UserName != exceptUser ).Select( r => {
                if ( user.Any( b => b.Hash == r.Hash ) ) return r;
                user.Add( r );
                return r;
            } ).ToList( );
            return user;
        }
        IList<IUserInfo> IHubConnectionContext<T>.GetActiveUser( string exceptUser, string hubname ) {
            IList<IUserInfo> user = new List<IUserInfo>( );
            _BaseClient.Where( w => w.HubName == hubname && w.UserName != exceptUser )
                .Select( r => {
                    if ( user.Any( b => b.user_name == r.UserName ) ) return r;
                    user.Add( new UserInfo {
                        user_name = r.UserName,
                        connection_id = r.ConnectionId,
                        hash = r.Hash,
                        time = r.Time
                    } );
                    return r;
                } ).ToList( );
            return user;
        }
        public IList<IUserInfo> GetActiveUser( params string[] exceptUser ) {
            IList<IUserInfo> user = new List<IUserInfo>( );
            ( from p in _BaseClient
              where p.UserName != ( exceptUser.FirstOrDefault( f => f == p.UserName ) )
              group p by p.UserName into g
              select g.Select( a => {
                  user.Add( new UserInfo {
                      user_name = a.UserName,
                      connection_id = a.ConnectionId,
                      hash = a.Hash,
                      time = a.Time
                  } );
                  return a;
              } ).ToList( ) ).ToList( );
            return user;
        }
        T IHubCallerContext<T>.Caller { get { return Caller; } }
        private dynamic AllInvoke( string[] excludeConnectionIds = null, string[] hubname = null ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( hubname != null ) {
                        if ( hubname.Any( f => f == a.HubName ) ) return false;
                    }
                    if ( excludeConnectionIds != null ) {
                        if ( excludeConnectionIds.Any( f => f == a.ConnectionId ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList();
                return Task.Delay( 0 );
            } );
        }
        T IHubConnectionContext<T>.All( params string[] hubname ) {
            return AllInvoke( new string[] { }, hubname );
        }
        public T All( ) {
            return AllInvoke(  );
        }
        public T AllExcept( string[] excludeConnectionIds, string[] hubname ) {
            return AllInvoke( excludeConnectionIds, hubname );
        }
        public T AllExcept( params string[] excludeConnectionIds ) {
            return AllInvoke( excludeConnectionIds );
        }
        public T AllExceptHash( params string[] excludeHashs ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( excludeHashs != null ) {
                        if ( excludeHashs.Any( b => b == a.Hash ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T AllExceptUser( string[] excludeUsers, string[] hubname ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( hubname != null ) {
                        if ( hubname.Any( f => f == a.HubName ) ) return false;
                    }
                    if ( excludeUsers != null ) {
                        if ( excludeUsers.Any( b => b == a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T AllExceptUser( params string[] excludeUsers ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( excludeUsers != null ) {
                        if ( excludeUsers.Any( b => b == a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }

        public ClientProxy ProxyAllExceptUser( string[] excludeUsers, string[] hubname ) {
            return new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( hubname != null ) {
                        if ( hubname.Any( f => f == a.HubName ) ) return false;
                    }
                    if ( excludeUsers != null ) {
                        if ( excludeUsers.Any( b => b == a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public ClientProxy ProxyAllExceptUser( params string[] excludeUsers ) {
            return new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( excludeUsers != null ) {
                        if ( excludeUsers.Any( b => b == a.UserName ) ) return false;
                    }
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }

        public T Client( string connectionId ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( f => f.ConnectionId == connectionId )
               .Select( a => {
                   a.Client.Invoke( m, arg );
                   return a;
               } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T Client( params string[] connectionId ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => a.ConnectionId == ( connectionId.FirstOrDefault( f => f == a.ConnectionId ) ) )
                .Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T ClientHash( string hash ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => a.Hash == hash ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T ClientHash( params string[] hash ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => a.ConnectionId == ( hash.FirstOrDefault( f => f == a.ConnectionId ) ) )
                .Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
       
        public T Clients( IList<string> connectionIds ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                connectionIds.Select( a => {
                    var k = _BaseClient.FirstOrDefault( f => f.ConnectionId == a );
                    if ( k == null ) return a;
                    k.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public IList<IHubContext> GetUserByHash(  string hash, string hubname ) {
           return _BaseClient.Where( a => a.HubName == hubname && a.Hash == hash ).ToList();
        }
        public string GetHash( string userName ) {
            var item = _BaseClient.FirstOrDefault( a => a.UserName == userName );
            if ( item == null ) return null;
            return item.Hash;
        }
        public IList<IHubContext> GetUserByHash( params string[] hash ) {
            return _BaseClient.Where( a => a.Hash == ( hash.FirstOrDefault( f => f == a.Hash ) ) ).ToList( );
        }
        public IList<IHubContext> GetUser( string userName, string hubname ) {
            return _BaseClient.Where( a => a.HubName == hubname && a.UserName == userName ).ToList( );
        }
        public IList<IHubContext> GetUser( params string[] userName ) {
            return _BaseClient.Where( a =>  a.UserName == ( userName.FirstOrDefault( f => f == a.UserName ) ) ).ToList( );
        }
        public IHubContext GetUserContext(string hash ) {
            return _BaseClient.FirstOrDefault( a => a.Hash == hash );
        }
        public IList<IHubContext> GetUserByConnectionId(  string connectionId, string hubname ) {
            return _BaseClient.Where( a => a.HubName == hubname && a.ConnectionId == connectionId ).ToList( );
        }
        public IList<IHubContext> GetUserByConnectionId( params string[] connectionId ) {
            return _BaseClient.Where( a =>  a.ConnectionId == ( connectionId.FirstOrDefault( f => f == a.ConnectionId ) ) ).ToList( );
        }
        public IList<IUserInfo> GetActiveHash( string exceptHash, string hubname ) {
            IList<IUserInfo> user = new List<IUserInfo>( );
            _BaseClient.Where( w => w.HubName == hubname && w.Hash != exceptHash ).Select( r => {
                if ( user.Any( b => b.hash == r.Hash ) ) return r;
                user.Add( new UserInfo {
                    user_name = r.UserName,
                    connection_id = r.ConnectionId,
                    hash = r.Hash,
                    time = r.Time
                } );
                return r;
            } ).ToList( );
            return user;
        }
        public IList<IUserInfo> GetActiveHash( params string[] exceptHash ) {
            IList<IUserInfo> user = new List<IUserInfo>( );
            _BaseClient.Where( p => p.Hash != ( exceptHash.FirstOrDefault( f => f == p.Hash ) ) )
                       .Select( a => {
                           if ( user.Any( b => b.hash == a.Hash ) ) return a;
                           user.Add( new UserInfo {
                               user_name = a.UserName,
                               connection_id = a.ConnectionId,
                               hash = a.Hash,
                               time = a.Time
                           } );
                           return a;
                       } ).ToList( );
            return user;
        }
        public T SendToUserByHash(  string hash, string hubname ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( a.HubName != hubname ) return false;
                    if ( a.Hash == hash ) return false;
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T SendToUserByHash( params string[] hash ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => a.Hash == ( hash.FirstOrDefault( f => f == a.Hash ) ) )
                .Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T SendToUserByName( string userName, string hubname ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( a.HubName != hubname ) return false;
                    if ( a.UserName == userName ) return false;
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T SendToUserByName( params string[] userName ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => a.Hash == ( userName.FirstOrDefault( f => f == a.UserName ) ) ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T SendToUserByConnectionId(  string connectionId, string hubname ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => {
                    if ( a.HubName != hubname ) return false;
                    if ( a.ConnectionId == connectionId ) return false;
                    return true;
                } ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }
        public T SendToUserByConnectionId( params string[] connectionId ) {
            return ( dynamic )new ClientProxy( ( m, arg ) => {
                _BaseClient.Where( a => a.ConnectionId == ( connectionId.FirstOrDefault( f => f == a.ConnectionId ) ) ).Select( a => {
                    a.Client.Invoke( m, arg );
                    return a;
                } ).ToList( );
                return Task.Delay( 0 );
            } );
        }

       
    }
}