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

namespace SOW.Web.Hub.Core.Handlers {
    internal class SendContext {
        public ArraySegment<byte> Buffer;
        public bool EndOfMessage;
        public WebSocketMessageType Type;
        public CancellationToken CancelToken;

        public SendContext( ArraySegment<byte> buffer, bool endOfMessage, WebSocketMessageType type, CancellationToken cancelToken ) {
            Buffer = buffer;
            EndOfMessage = endOfMessage;
            Type = type;
            CancelToken = cancelToken;
        }
    }
}