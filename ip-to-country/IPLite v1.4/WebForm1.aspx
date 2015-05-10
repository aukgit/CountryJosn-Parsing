<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btCheckCurrent" runat="server" Text="Check Current Visitor's IP" OnClick="btCheckCurrent_Click" />
        <br /><br />
        <asp:Button ID="btManualCheck" runat="server" Text="Check This IP Address" OnClick="btManualCheck_Click" />
    &nbsp;<asp:TextBox ID="TextBox1" runat="server" Width="175px"></asp:TextBox>
    </div>
    </form>
</body>
</html>
