<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallNavigationViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Details</h2>

     <fieldset>
    <legend><%: Html.Encode(Model.CallForProposal.Name) %> </legend>
        <%: Html.HtmlEncode(Model.CallForProposal.Description) %>
    </fieldset>

    <fieldset>
        <legend>Details</legend>
        <ul>
        <li>
            <%: Html.CheckBoxFor(a => a.CallForProposal.IsActive, new {@disabled = "true"}) %> <%: Html.Encode("Active") %>
        </li>
        <li>
            <%: Html.Label("Created Date:")%>
            <%: String.Format("{0:g}", Model.CallForProposal.CreatedDate)%>
        </li>
        <li>
            <%: Html.Label("End Date:")%>
            <%: String.Format("{0:g}", Model.CallForProposal.EndDate)%>
        </li>
        <li>
            <%: Html.Label("Calls Sent Date:")%>
            <%: String.Format("{0:g}", Model.CallForProposal.CallsSentDate)%>
        </li>
        </ul>
    </fieldset>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

