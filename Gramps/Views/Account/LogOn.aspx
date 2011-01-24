<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content Id="Content1" ContentPlaceHolderId="TitleContent" runat="server">
	Access Denied
</asp:Content>

<asp:Content Id="Content2" ContentPlaceHolderId="MainContent" runat="server">

    <h2>Access Denied</h2>

    <p>
        You do not have access to view this page: <%= Html.Encode(TempData["URL"]) %>
    </p>
    
</asp:Content>
