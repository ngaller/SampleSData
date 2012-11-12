<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
<tr>
    <td>SData URL:</td>
    <td>
        <asp:TextBox runat="server" id="txtUrl" Text="http://sssngaller3.sssworld-local.com:1080/sdata" />
        <asp:RegularExpressionValidator runat="server" Text="*" ControlToValidate="txtUrl" ValidationExpression="^https?://.+/sdata$" />
    </td>
</tr>
<tr>
    <td>User name:</td>
    <td><asp:TextBox runat="server" id="txtUsername" Text="ADMIN" /><asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername" Text="*" /></td>
</tr>
<tr>
    <td>Password:</td>
    <td><asp:TextBox TextMode="Password" runat="server" id="txtPassword" value="" /></td>
</tr>
<tr>
    <td># Accounts:</td>
    <td><asp:TextBox runat="server" ID="txtAccounts" Text="10" /></td>
</tr>
<tr>
    <td># Opportunities:</td>
    <td><asp:TextBox ToolTip="Number of opportunities to create for each account" runat="server" ID="txtOpportunities" Text="10" /></td>
</tr>
<tr>
    <td># Contacts:</td>
    <td><asp:TextBox runat="server" ID="txtContacts" Text="10" ToolTip="Number of contacts to create for each account" /></td>
</tr>
<tr>
    <td colspan="2" style="text-align: right">
        <asp:Button runat="server" Text="Run Sample" ID="btnRunSample" />
    </td>
</tr>
<tr>
    <td colspan="2">
        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtResults" Rows="10" />
    </td>
</tr>
</table>
    </div>
    </form>
</body>
</html>
