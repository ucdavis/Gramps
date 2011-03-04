<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%= Html.ActionLink<CallForProposalController>(a => a.Index(null), "Calls Index")%>
        </li>
        <li>
            <%= Html.ActionLink<TemplateController>(a => a.Index(), "Template Index")%>
        </li>

    </ul>
</asp:Content>
