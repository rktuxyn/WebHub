//Thanks to ==> https://github.com/bryceg/Owin.WebSocket
/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SOW.Web.Hub.Core.Extensions;

namespace SOW.Web.Hub.Core.Handlers {
    public interface IWebSocket {
        TaskQueue SendQueue { get; }
        Task SendText( ArraySegment<byte> data, bool endOfMessage, CancellationToken cancelToken );
        Task SendBinary( ArraySegment<byte> data, bool endOfMessage, CancellationToken cancelToken );
        Task Send( ArraySegment<byte> data, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancelToken );
        Task Close( WebSocketCloseStatus closeStatus, string closeDescription, CancellationToken cancelToken );
        Task<Tuple<ArraySegment<byte>, WebSocketMessageType>> ReceiveMessage( byte[] buffer, CancellationToken cancelToken );
        WebSocketCloseStatus? CloseStatus { get; }
        string CloseStatusDescription { get; }
    }
}