/**
* Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
* @author {SOW}
* Copyrights licensed under the New BSD License.
* See the accompanying LICENSE file for terms.
*/
namespace SOW.Web.Hub.View {
    public interface IMessageDetail {
        string to_user_hash { get; set; }
        string from_user_hash { get; set; }
        string message { get; set; }
        string msg_date { get; set; }
    }
    public class MessageDetail : IMessageDetail {
        public string to_user_hash { get; set; }
        public string from_user_hash { get; set; }
        public string message { get; set; }
        public string msg_date { get; set; }
    }
    public interface IPublicMessageDetail {
        string publish_hash { get; set; }
        string publisher_name { get; set; }
        string message { get; set; }
        string msg_date { get; set; }
    }
    public class PublicMessageDetail : IPublicMessageDetail {
        public string publish_hash { get; set; }
        public string publisher_name { get; set; }
        public string message { get; set; }
        public string msg_date { get; set; }
    }
    
}
