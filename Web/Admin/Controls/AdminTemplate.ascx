<%@ Control Language="c#" AutoEventWireup="false" Codebehind="AdminTemplate.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.AdminTemplate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Navigation" Src="Navigation.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 TRANSITIONAL//EN" >
<html>
	<head>
		<title><asp:literal id="PageTitle" runat="server"></asp:literal></title>
		<link id="CssStyleSheet" rel="stylesheet" type="text/css" runat="server" />
	</head>
	<body>
		<form id="Frm" method="post" runat="server" enctype="multipart/form-data">
			<uc1:header id="Header" runat="server"></uc1:header>
			<div id="menupane">
				<uc1:navigation id="Nav" runat="server"></uc1:navigation>
			</div>
			<div id="contentpane">
				<h1><asp:literal id="PageTitleLabel" runat="server" /></h1>
				<div id="MessageBox" class="messagebox" runat="server" visible="false" enableviewstate="false"></div>
				<asp:placeholder id="PageContent" runat="server"></asp:placeholder>
			</div>
		</form>

	</body>
</html>