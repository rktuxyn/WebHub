/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.Core {
    using System;
    using System.Threading.Tasks;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Reflection.Emit;

    public class ClientProxy : DynamicObject {
        private readonly Func<string, object[], Task> _invoker;
        public ClientProxy( Func<string, object[], Task> invoker ) { _invoker = invoker; }
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Binder is passed in by the DLR" )]
        public override bool TryInvokeMember( InvokeMemberBinder binder, object[] args, out object result ) {
            result = Invoke( binder.Name, args );
            return true;
        }
        public Task proxyInvoke(string name, object[] args ) {
           return Invoke( name, args );
        }
        public Task Invoke( string method, object[] args ) {
            //List<object> arg = new List<object>();
            //arg.Add( method ); arg.Add( args );
            return _invoker.Invoke( method, args );
            //return Task.Delay( 0 );
        }
        /**public Task Invoke( string method, object[] args ) {
            //List<object> arg = new List<object>();
            //arg.Add( method ); arg.Add( args );
            return _invoker.Invoke( method, args );
            //return Task.Delay( 0 );
        }*/
    }
}
