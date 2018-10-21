/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using Microsoft.Owin;
    using SOW.Web.Hub.Core.Extensions;
    using SOW.Web.Hub.Core.Handlers;
    public abstract class Hubs : IHubs, IDisposable {
        public static IHubConfiguration HubConfig { get; set; }
        private readonly CancellationTokenSource mCancellToken;

        private IWebSocket _mWebSocket { get; set; }

        private string _ConnectionId {
            get;
            set;
        }
        private string _Hash { get; set; }
        public string Hash {
            get {
                return _Hash;
            }
        }
        private IOwinContext _Context {
            get;
            set;
        }

        public JavaScriptSerializer _jss {
            get;
            private set;
        }

        public Dictionary<string, string> Arguments {
            get;
            private set;
        }

        public IHubCallerContext<dynamic> Clients {
            get;
            set;
        }

        public string ConnectionId {
            get {
                return this._ConnectionId;
            }
        }

        public IOwinContext Context {
            get {
                return this._Context;
            }
        }

        public string hubName {
            get;
            private set;
        }

        public int MaxMessageSize {
            get;
            private set;
        }

        public IWebSocket mWebSocket {
            get {
                return this._mWebSocket;
            }
        }

        public TaskQueue QueueSend {
            get {
                return this._mWebSocket.SendQueue;
            }
        }

        public IOwinRequest Request {
            get {
                return this._Context.Request;
            }
        }

        public IOwinResponse Response {
            get {
                return this._Context.Response;
            }
        }
        public bool IsAuthenticated { get; private set; }
        public IPrincipal User {
            get {
                return this._Context.Request.User;
            }
        }
        private bool IsReconnect { get; set; }
        public string UserName { get; private set; }
        protected Hubs( int maxMessageSize = 1024 * 64 ) {
            this.mCancellToken = new CancellationTokenSource( );
            this.MaxMessageSize = maxMessageSize;
            this._jss = new JavaScriptSerializer( ) {
                MaxJsonLength = 2147483647
            };
            this.IsReconnect = false;
            this.Clients = new HubCallerContext<object>( );
        }
        private Task OnMessageReceived( ArraySegment<byte> message, WebSocketMessageType type ) {
            if ( WebSocketMessageType.Binary == type ) {
                return Task.Delay( 0 );
            }
            string json = Encoding.UTF8.GetString( message.Array, message.Offset, message.Count );
            try {
                IMessage info = this._jss.Deserialize<Message>( json );
                if ( info == null ) {
                    return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                        H = this.hubName,
                        M = "onError",
                        A = new object[] { "Message Required!!!", json },
                        T = this.ConnectionId,
                        HS = this.Hash
                    } ) ), true );
                }
                if ( !string.IsNullOrEmpty( info.M ) ) {
                    Type child = Util.GetType( info.H, this.GetType( ) );
                    MethodInfo method = child.GetMethod( info.M, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public );
                    if ( method == null ) {
                        return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                            H = this.hubName,
                            M = "onError",
                            A = new object[] { string.Format( "Invalid Method defined==>{0}!!!", info.M ), json },
                            T = this.ConnectionId,
                            HS = this.Hash
                        } ) ), true );
                    }
                    IEnumerable<Attribute> myAttributes = method.GetCustomAttributes( typeof( AuthorizeAttribute ) );
                    if ( myAttributes != null ) {
                        bool unath = false;
                        foreach ( Attribute aa in myAttributes ) {
                            if ( !( aa is AuthorizeAttribute ) ) {
                                break;
                            }
                            AuthorizeAttribute rAttribute = ( AuthorizeAttribute )aa;
                            if ( rAttribute.IsDefaultAttribute( ) ) {
                                if ( rAttribute.IsAuthorized( this.Context.Request ) ) {
                                    break;
                                }
                                unath = true;
                                break;
                            }
                            if ( !rAttribute.IsInRole( this.Context.Request ) ) {
                                unath = true;
                                break;
                            }
                            if ( !rAttribute.IsInUsers( this.Context.Request ) ) {
                                unath = true;
                                break;
                            }
                        }
                        if ( unath ) {
                            return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                                H = this.hubName,
                                M = method.Name,
                                A = new object[] { string.Format( "You should not access this method without authorization==>{0}!!!", info.M ), json },
                                T = this.ConnectionId,
                                HS = this.Hash
                            } ) ), true );
                        }
                        myAttributes = method.GetCustomAttributes( typeof( InternalAttribute ) );
                        if ( myAttributes != null ) {
                            return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                                H = this.hubName,
                                M = method.Name,
                                A = new object[] { string.Format( "You should not access this method==>{0}!!!", info.M ), json },
                                T = this.ConnectionId,
                                HS = this.Hash
                            } ) ), true );
                        }
                    }
                    if ( method.GetParameters( ).Length != info.A.Length ) {
                        return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                            H = this.hubName,
                            M = method.Name,
                            A = new object[] { string.Format( "Given argumnets not match with =>{0}", info.M ), json },
                            T = this.ConnectionId,
                            HS = this.Hash
                        } ) ), true );
                    }
                    return ( Task )method.Invoke( this, info.A );

                }
                return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                    H = this.hubName,
                    M = "onError",
                    A = new object[] { "Method Required!!!", json },
                    T = this.ConnectionId,
                    HS = this.Hash
                } ) ), true );
            } catch ( Exception exception ) {
                return this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                    H = this.hubName,
                    M = "onError",
                    A = new object[] { "Runtime error!!!", json },
                    T = this.ConnectionId,
                    D = HubConfig.EnableJavaScriptProxies == true ? exception.Message : null,
                    HS = this.Hash
                } ) ), true );
            }
        }

        public void Abort( ) {
            this.OnDisconnected( true );
            this.mCancellToken.Cancel( );
        }
        internal async Task ProcessInternalRequest( ) {
            try {
                var json = Context.Request.Query["def"];
                if ( string.IsNullOrEmpty( json ) ) {
                    Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    await Context.Response.WriteAsync( "Not Found!!!" );
                    return;
                }
                IMessage info = this._jss.Deserialize<Message>( json );
                if ( string.IsNullOrEmpty( info.M ) ) {
                    Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    await Context.Response.WriteAsync( "Not Found!!!" );
                    return;
                }
                Type child = Util.GetType( info.H, this.GetType( ) );
                MethodInfo method = child.GetMethod( info.M, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public );
                if ( method == null ) {
                    Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    await Context.Response.WriteAsync( "Not Found!!!" );
                    return;
                }
                if ( method.GetParameters( ).Length != info.A.Length ) {
                    Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                    await Context.Response.WriteAsync( "Not Found!!!" );
                    return;
                }
                IEnumerable<Attribute> myAttributes = method.GetCustomAttributes( typeof( InternalAttribute ) );
                foreach ( Attribute aa in myAttributes ) {
                    if ( !( aa is InternalAttribute ) ) {
                        Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                        await Context.Response.WriteAsync( "Not Found!!!" );
                        return;
                    }
                    InternalAttribute rAttribute = ( InternalAttribute )aa;
                    if ( rAttribute.Internal != info.M ) {
                        Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                        await Context.Response.WriteAsync( "Not Found!!!" );
                        return;
                    }
                }
                await ( Task )method.Invoke( this, info.A );
                Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.OK;
                //await Context.Response.WriteAsync( _jss.Serialize( info.A ) );
                await Context.Response.WriteAsync( "SUCCESS" );
            } catch {
                Context.Response.StatusCode = ( int )System.Net.HttpStatusCode.BadRequest;
                await Context.Response.WriteAsync( "BadRequest" );
            }
            return;
        }
        internal async Task AcceptSocketAsync( IUrlReq request, bool isReconnect, IOwinContext context, IDictionary<string, string> argumentMatches ) {
            this.IsReconnect = isReconnect;
            if ( context.Request.User != null ) {
                if ( context.Request.User.Identity != null ) {
                    this.IsAuthenticated = context.Request.User.Identity.IsAuthenticated;
                    this.UserName = context.Request.User.Identity.Name;
                }
            }
            string value = context.Request.Path.Value;
            if ( request == null ) {
                context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                await context.Response.WriteAsync( string.Format( "Not Found ==> {0}", value ) );
                return;
            }
            Type type;
            type = Util.GetType( request.H, this.GetType( ) );
            if ( type == null ) {
                await context.Response.WriteAsync( string.Format( "Not Found ==> {0}", value ?? "/" ) );
                context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                return;
            }
            if ( type.Name != request.H ) {
                await context.Response.WriteAsync( string.Format( "Not Found ==> {0}", value ?? "/" ) );
                context.Response.StatusCode = ( int )System.Net.HttpStatusCode.NotFound;
                return;
            }
            this.hubName = request.H;
            if ( argumentMatches is null && request.M == "__internal__" ) {
                this._ConnectionId = Guid.NewGuid( ).ToString( "N" );
                this._Context = context;
                await this.ProcessInternalRequest( );
                return;
            }
            var accept = context.Get<Action<IDictionary<string, object>, Func<IDictionary<string, object>, Task>>>( "websocket.Accept" );
            if ( accept == null ) {
                context.Response.StatusCode = ( int )System.Net.HttpStatusCode.BadRequest;
                context.Response.Write( "Not a valid websocket request" );
                return;
            }
            this._ConnectionId = Guid.NewGuid( ).ToString( "N" );

            Arguments = new Dictionary<string, string>( argumentMatches );

            context.Environment.Get<Action>( "server.DisableResponseBuffering" )?.Invoke( );

            context.Environment.Get<Action>( "systemweb.DisableResponseCompression" )?.Invoke( );

            context.Response.Headers.Set( "X-Content-Type-Options", "nosniff" );

            context.Response.Headers.Set( "X-Content-Type-Options", "nosniff" );
            context.Response.Headers.Set( "X-Powered-By", "https://www.safeonlineworld.com" );
            this._Context = context;
            if ( this.AuthenticateRequest( ) ) {
                accept( null, new Func<IDictionary<string, object>, Task>( this.RunWebSocket ) );
                return;
            }
            if ( !await this.AuthenticateRequestAsync( ) ) {
                bool flag = ( context.Request.User == null ? false : context.Request.User.Identity.IsAuthenticated );
                if ( !flag ) {
                    context.Response.StatusCode = 401;
                    return;
                }
                context.Response.StatusCode = 403;
                return;
            }
            accept( null, new Func<IDictionary<string, object>, Task>( this.RunWebSocket ) );
            return;
        }

        public virtual bool AuthenticateRequest( ) {
            return true;
        }

        public virtual Task<bool> AuthenticateRequestAsync( ) {
            return Task.FromResult<bool>( true );
        }

        public Task Close( WebSocketCloseStatus status, string reason ) {
            this.OnDisconnected( true );
            return this._mWebSocket.Close( status, reason, CancellationToken.None );
        }

        internal static bool IsFatalSocketException( Exception ex ) {
            // If this exception is due to the underlying TCP connection going away, treat as a normal close
            // rather than a fatal exception.
            var ce = ex as COMException;
            if ( ce != null ) {
                switch ( ( uint )ce.ErrorCode ) {
                    case 0x800703e3:
                    case 0x800704cd:
                    case 0x80070026:
                        return false;
                }
            }
            // unknown exception; treat as fatal
            return true;
        }

        public virtual Task OnConnected( ) {
            return ( Task )( ( dynamic )Clients.Caller ).onConnected( ConnectionId, Hash, UserName, Clients.GetActiveHash( Hash ) );
        }

        public virtual Task OnDisconnected( bool stopCalled ) {
            this.Clients.Remove( new string[] { this.ConnectionId } );
            return Task.Delay( 0 );
        }
        public Task Ok( ) {
            return Task.Delay( 0 );
        }
        public virtual void OnReceiveError( Exception error ) {
        }

        public virtual Task OnReconnected( ) {
            return ( Task )( ( dynamic )Clients.Caller ).onReconnected( Hash, ConnectionId, UserName );
        }

        private async Task RunWebSocket( IDictionary<string, object> websocketContext ) {
            object obj;
            if ( !websocketContext.TryGetValue( typeof( WebSocketContext ).FullName, out obj ) ) {
                this._mWebSocket = new OwinWebSocket( websocketContext );
            } else {
                this._mWebSocket = new NetWebSocket( ( ( WebSocketContext )obj ).WebSocket );
            }
            if ( this.IsAuthenticated ) {
                if ( !this.Clients.Exists( User.Identity.Name ) ) {
                    this._Hash = Util.GetHash( User.Identity.Name );
                } else {
                    this._Hash = this.Clients.GetHash( User.Identity.Name );
                }
            } else {
                this._Hash = Util.GetHash( this.ConnectionId );
            }

            this.Clients.Set( new HubContext( ) {
                Time = DateTime.UtcNow.ToString( ),
                HubName = this.hubName,
                ConnectionId = this.ConnectionId,
                UserName = !this.IsAuthenticated ? null : User.Identity.Name,
                Client = new ClientProxy( ( string method, object[] args ) => this.SendText( Encoding.UTF8.GetBytes( this._jss.Serialize( new Message( ) {
                    H = this.hubName,
                    A = args,
                    M = method,
                    T = this.ConnectionId,
                    HS = this.Hash
                } ) ), true ) ),
                IsAdmin = !this.IsAuthenticated ? false : ( this.Context.Request.User == null ? false : this.Context.Request.User.IsInRole( "Admin" ) ),
                Hash = this.Hash
            } );
            if ( this.IsReconnect ) {
                await this.OnReconnected( );
            } else {
                await this.OnConnected( );
            }
            byte[] numArray = new byte[this.MaxMessageSize];
            Tuple<ArraySegment<byte>, WebSocketMessageType> tuple = null;
            do {
                try {
                    Tuple<ArraySegment<byte>, WebSocketMessageType> tuple1 = await this._mWebSocket.ReceiveMessage( numArray, this.mCancellToken.Token );
                    tuple = tuple1;
                    tuple1 = null;
                    if ( tuple.Item1.Count > 0 ) {
                        await this.OnMessageReceived( tuple.Item1, tuple.Item2 );
                    }
                } catch ( TaskCanceledException taskCanceledException ) {
                    if ( Environment.UserInteractive ) {
                        Console.WriteLine( taskCanceledException.Message );
                    }
                    break;
                } catch ( OperationCanceledException operationCanceledException ) {
                    if ( !this.mCancellToken.IsCancellationRequested ) {
                        this.OnReceiveError( operationCanceledException );
                    }
                    break;
                } catch ( ObjectDisposedException objectDisposedException ) {
                    if ( Environment.UserInteractive ) {
                        Console.WriteLine( objectDisposedException.Message );
                    }
                    break;
                } catch ( Exception exception ) {
                    if ( Hubs.IsFatalSocketException( exception ) ) {
                        this.OnReceiveError( exception );
                    }
                    break;
                }
            }
            while ( tuple.Item2 != WebSocketMessageType.Close );
            try {
                await this._mWebSocket.Close( WebSocketCloseStatus.NormalClosure, string.Empty, this.mCancellToken.Token );
            } catch { }
            if ( !this.mCancellToken.IsCancellationRequested ) {
                this.mCancellToken.Cancel( );
            }
            await this.OnDisconnected( true );
        }

        public Task Send( ArraySegment<byte> buffer, bool endOfMessage, WebSocketMessageType type ) {
            Task task = this._mWebSocket.Send( buffer, type, endOfMessage, this.mCancellToken.Token );
            return task;
        }

        public Task SendBinary( byte[] buffer, bool endOfMessage ) {
            return this.SendBinary( new ArraySegment<byte>( buffer ), endOfMessage );
        }

        public Task SendBinary( ArraySegment<byte> buffer, bool endOfMessage ) {
            Task task = this._mWebSocket.SendBinary( buffer, endOfMessage, this.mCancellToken.Token );
            return task;
        }

        public Task SendText( byte[] buffer, bool endOfMessage = true ) {
            return this.SendText( new ArraySegment<byte>( buffer ), endOfMessage );
        }

        public Task SendText( ArraySegment<byte> buffer, bool endOfMessage ) {
            Task task = this._mWebSocket.SendText( buffer, endOfMessage, this.mCancellToken.Token );
            return task;
        }
        ~Hubs( ) { }
        public void Dispose( ) {
            GC.SuppressFinalize( this );
            GC.Collect( 0, GCCollectionMode.Optimized );
        }
    }
}