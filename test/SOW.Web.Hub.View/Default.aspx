<!--
	/**
	    * Copyright (c) 2018, SOW (https://www.facebook.com/safeonlineworld). (https://github.com/RKTUXYN) All rights reserved.
	    * Copyrights licensed under the New BSD License.
	    * See the accompanying LICENSE file for terms.
	*/
-->
<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="css/bootstrap.css" />
    <style type="text/css">
        <!--
        .board{
            /*max-height: 32em;
            overflow: auto;*/
        }
        .servernote{
            padding-top: 5px;
            padding-bottom: 5px;
        }
        .panel-body{
            max-height: 400px;
            overflow: auto;
        }
        .panel-body.board-wrapper{
            min-height:400px;
        }
        .to_message {
            float: left;
            margin-top: 0;
            white-space: pre-wrap;
            word-wrap: break-word;
            display: block;
            clear: both;
            padding: 0px 5px 0px 5px;
        }
        .from_message {
            color:#337ab7;
            float: right;
            margin-top: 0;
            white-space: pre-wrap;
            word-wrap: break-word;
            display: block;
            clear: both;
            padding: 0px 5px 0px 5px;
        }
        .disabled {
          pointer-events: none;
          cursor: default;
          opacity: 0.6;
        }
        .well {
            min-height: 20px;
            padding: 5px;
            margin-bottom: 20px;
            background-color: #03f5900f;
            border: 1px solid #bce8f1;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.05);
            box-shadow: inset 0 1px 1px rgba(0,0,0,.05);
        }
        .chat_box {
            /*position: absolute;*/
            border: #bce8f1 solid 1px !important;
            width: 100%;
        }

        .chat_box .header {
            /*cursor: move;*/
            background-color: #d9edf7;
            border-bottom: #bce8f1 solid 1px;
            color: #bce8f1;
        }

        .chat_box .selText {
            color: black;
            padding: 4px;
        }

        .chat_box .messageArea {
            overflow-y: auto;
            border-bottom: #bce8f1 solid 1px;
            min-height: 80px;
            max-height: 150px;
        }

        .chat_box .messageArea .message {
            padding: 4px;
        }

        .chat_box .buttonBar {
            /*width: 250px;*/
            padding: 4px;
        }

        .chat_box .buttonBar .msgText {
            width: 80%;
            border: #bce8f1 1px solid;
            height: 30px;
            padding-left: 10px;
        }

        .chat_box .buttonBar .button {
            margin-left: 4px;
            width: 55px;
            border: #bce8f1 1px solid;
        }
        @media (min-width: 1200px){
            .container {
                width: 100%;
            }
        }
        table{
            width:100%;
        }
        -->
    </style>
</head>
<body>
    <div class="container">
        <div class="row">
            <div class="col-sm-12" style="font-weight:bold; text-align:center;">
                <div class="panel panel-info">
                     <div class="panel-heading">
                          Chat User Name : <%= Context.User.Identity.Name %>
                      </div>
                </div>
            </div>
          </div>
        <div class="row">
          <div class="col-md-2 col-sm-12">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        ONLINE USERS
                    </div>
                    <div class="panel-body">
                    <ul class="media-list connected-list" data-chat="user">
                        <!--[User List]-->
                    </ul>
                    <hr />
                    </div>
                </div>
           </div>
            <div class="col-md-5 col-sm-12" data-chat="message-public">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        PUBLIC MESSAGE BOARD
                    </div>
                    <div class="panel-body board-wrapper">
                        <ul class="media-list board">

                        </ul>
                    </div>
                    <div class="panel-footer">
                        <form class="input-group">
                            <input disabled="disabled" id="m" type="text" class="form-control" placeholder="Enter Message" />
                            <span class="input-group-btn">
                            <input type="button" class="btn btn-info send" value="Send" disabled="disabled"/>
                            </span>
                        </form>
                    </div>
                </div>
            </div>
            <div class="col-md-5 col-sm-12">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        PRIVATE MESSAGE BOARD
                    </div>
                    <div class="panel-body board-wrapper" data-chat="message-private">
                        <table style="width:100%"><tbody></tbody></table>
                    </div>
                    <div class="panel-footer" style="height: 55px;"></div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/template" data-type="message-template">
       <div class="col-sm-4 chat_box" style="float:left; padding-right:unset!important; padding-left:unset!important">
        <div data-user="{0}">
           <div class="header">
              <span class="selText">{1}</span>
               <span class="close">x</span>
           </div>
           <div class="messageArea"></div>
           <div class="buttonBar">
              <input class="msgText txtPrivateMessage" placeholder="Enter Message" type="text"/>
              <input class="submitButton button btnSendMessage btn btn-info send" type="button" value="Send"   />
           </div>
        </div>
       </div>
    </script>
    <script src="/Js/jQuery/3.2.1/jquery.min.js"></script>
    <script src="/Js/sow.api.hub.js"></script>
    <script src="/hub/Manager/proxy/"></script>
    <script src="/Js/hub.demo.js"></script>
</body>
</html>
