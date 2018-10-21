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
    using System.Text;
    using System.Security.Cryptography;
    public interface IMessage {
        string H { get; set; }
        string M { get; set; }
        object[] A { get; set; }
        string T { get; set; }
        string D { get; set; }
        string HS { get; set; }
    }
    [System.Serializable]
    public class Message : IMessage {
        public string H { get; set; }
        public string M { get; set; }
        public object[] A { get; set; }
        public string T { get; set; }
        public string D { get; set; }
        public string HS { get; set; }
    }
    public interface IUserInfo {
        string hash { get; set; }
        string connection_id { get; set; }
        string user_name { get; set; }
        string time { get; set; }
    }
    public class UserInfo : IUserInfo {
        public string hash { get; set; }
        public string connection_id { get; set; }
        public string user_name { get; set; }
        public string time { get; set; }
    }
    interface IUrlReq {
        string H { get; set; }
        string M { get; set; }
    }
    public class UrlReq : IUrlReq {
        public string H { get; set; }
        public string M { get; set; }
    }
    class Util {
        public static Type GetType( string Name, Type types ) {
            IEnumerable<Type> subclasses = types.Assembly.GetTypes( ).Where( type => type.IsSubclassOf( typeof( Hubs ) ) );
            //IEnumerable<Type> subclasses = types.Where( tx => tx.IsSubclassOf( parentType ) );
            return subclasses.FirstOrDefault( a => a.Name == Name );
            //return types;
        }
        public static string GetHash( string key ) {
            StringBuilder hash = new StringBuilder( );
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider( );
            byte[] bytes = md5provider.ComputeHash( new UTF8Encoding( ).GetBytes( key ) );
            bytes.Select( a => {
                hash.Append( a.ToString( "x2" ) );
                return a;
            } ).ToList( );
            return hash.ToString( );
        }
        public static IUrlReq GetRequest( string reqPath ) {
            if ( string.IsNullOrEmpty( reqPath ) ) return null;
            var arr = reqPath.Split( '/' );
            if ( arr == null ) return null;
            if ( arr.Length < 2 ) return null;
            IList<string> parts = new List<string>( );
            for ( int x = 0, l = arr.Length ; x < l ; x++ ) {
                if ( string.IsNullOrEmpty( arr[x] ) ) continue;
                parts.Add( arr[x].Trim() );
            }
            if ( parts.Count < 2 ) return null;
            //string hub = lost[0];
            /**if ( Hubs.HubConfig.StandardHubName ) {
                hub = Char.ToUpperInvariant( hub[0] ) + hub.Substring( 1 );
            }*/
            return new UrlReq {
                H = parts[0],
                M = parts[1]
            };
        }
    }
}
