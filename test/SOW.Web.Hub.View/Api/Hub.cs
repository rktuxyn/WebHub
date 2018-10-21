/*
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld).  All rights reserved.
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.View {
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using SOW.Web.Hub.Core;

    public class Manager : Hubs {
        public object _locker = new object( );
        static IList<IMessageDetail> _privateMessage = new List<IMessageDetail>( );
        static IList<IPublicMessageDetail> _publicMessage = new List<IPublicMessageDetail>( );
        public override Task OnConnected( ) {
            ( ( dynamic )Clients.AllExceptHash( base.Hash ) ).onNewUserConnected( base.Hash, base.ConnectionId, base.UserName, DateTime.Now.ToString( ) );
            return base.OnConnected( );
        }
        public override Task OnDisconnected( bool stopCalled ) {
            ( ( dynamic )Clients.AllExceptHash( base.Hash ) ).onDisconnectUser( base.Hash, base.UserName );
            return base.OnDisconnected( stopCalled );
        }
        public override Task OnReconnected( ) {
            ( ( dynamic )Clients.AllExceptHash( base.Hash ) ).onReconnected( base.Hash, base.ConnectionId, base.UserName, DateTime.Now.ToString( ) );
            return base.OnReconnected( );
        }
        public override bool AuthenticateRequest( ) {
            return base.AuthenticateRequest( );
        }
        public override Task<bool> AuthenticateRequestAsync( ) {
            return base.AuthenticateRequestAsync( );
        }
        [Authorize]
        public Task Connect( string pageUri ) {
            return Ok( );
        }
        /*[Load User]*/
        [Authorize( Roles = "Team, Admin" )]
        public Task LoadMembar( string seq ) {
            return ( Task )( ( dynamic )Clients.Caller ).onLoadMembar( seq, _jss.Serialize(base.Clients.GetActiveHash( ) ) );
        }
        #region Global Message
        [Authorize( Roles = "Team, Admin" )]
        public Task GlobalMessageKeyup( string message ) {
            return ( Task )( ( dynamic )Clients.AllExceptHash( base.Hash ) ).onGlobalMessageKeyup( base.Hash, message );
        }
        [Authorize( Roles = "Team, Admin" )]
        public Task LoadPublicMessage(  ) {

            return ( Task )( ( dynamic )Clients.Caller ).onLoadPublicMessage( _jss.Serialize( _publicMessage ) );
        }
        public Task SendPublicMessage( string message ) {
            return ( Task )( ( dynamic )Clients.All( ) ).onPublicMessage( _jss.Serialize( this.AddPublicMessage( message ) ) );
        }
        private IPublicMessageDetail AddPublicMessage( string message ) {
            if ( _publicMessage.Count > 100 ) {
                lock ( _locker ) {
                    _publicMessage.Clear( );
                }
            }
            IPublicMessageDetail md = new PublicMessageDetail {
                publish_hash = Hash,
                publisher_name = UserName,
                message = message,
                msg_date = DateTime.Now.ToString( )
            };
            lock ( _locker ) {
                _publicMessage.Add( md );
            }
            return md;
        }
        #endregion Global Message
        #region Private Message
        [Authorize( Roles = "Team, Admin" )]
        public Task SendPrivateMessage( string toHash, string message ) {
            this.AddPrivateMessage( toHash, message );
            return ( Task )( ( dynamic )Clients.ClientHash( toHash ) ).onPrivateMessage( Hash, UserName, message );
        }
        [Authorize( Roles = "Team, Admin" )]
        public Task LoadPrivateMessage( string toHash ) {
            return ( Task )( ( dynamic )Clients.Caller ).onLoadPrivateMessage( toHash, _jss.Serialize( this.GetPrivateMessage( toHash ) ) );
        }
        [Authorize( Roles = "Team, Admin" )]
        public Task PrivateMessageKeyup( string toHash ) {
            return ( Task )( ( dynamic )Clients.ClientHash( toHash ) ).onPrivateMessageKeyup( Hash ); ;
        }
        private IList<IMessageDetail> GetPrivateMessage( string hash ) {
            return _privateMessage.Where( a => ( a.from_user_hash == base.Hash && a.to_user_hash == hash ) || ( a.from_user_hash == hash && a.to_user_hash == base.Hash ) ).ToList( );
        }
        private IMessageDetail AddPrivateMessage( string toHash, string message ) {
            if ( _privateMessage.Count > 100 ) {
                lock( _locker ) {
                    _privateMessage.Clear( );
                }
            }
            IMessageDetail messageDetail = new MessageDetail {
                to_user_hash = toHash,
                from_user_hash = Hash,
                message = message,
                msg_date = DateTime.Now.ToString( )
            };
            lock ( _locker ) {
                _privateMessage.Add( messageDetail );
            }
            return messageDetail;
        }
        #endregion  Private Message
       
    }
}
