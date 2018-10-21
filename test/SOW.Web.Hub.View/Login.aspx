<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SOW.Web.Hub.View.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <span id="message" runat="server"></span><br />
       Chat User Name: <input type="text" id="chat_user" runat="server" />
        <select runat="server" id="group_type" disabled="disabled">
            <option>Select...</option>
            <option value="Admin" selected="selected" >Admin</option>
            <option value="Team" >Admin</option>
        </select>
       <input type="button" runat="server" value="Authenticate" name="Authenticate" onserverclick="Authenticate_Click"/>
    </form>
</body>
</html>
