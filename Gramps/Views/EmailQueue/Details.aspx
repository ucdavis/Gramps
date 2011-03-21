<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailQueueViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <ul>
        <li>
            <%= Html.CheckBox("Pending", Model.EmailQueue.Pending, new { @disabled = "True" })%> <%: Html.Encode("Pending") %>
        </li>
        <li>
            <%= Html.CheckBox("Immediate", Model.EmailQueue.Immediate, new { @disabled = "True" })%> <%: Html.Encode("Immediate")%>
        </li>
        <li>
            <%: Html.Label("Email Address:")%>
            <%: Model.EmailQueue.EmailAddress %>
        </li>
        <li>
            <%: Html.Label("Created:")%>
            <%: String.Format("{0:g}", Model.EmailQueue.Created)%>   
        </li>
        <li>
            <%: Html.Label("Sent Date Time:")%>
            <%: String.Format("{0:g}", Model.EmailQueue.SentDateTime)%>
        </li>
        <li>
            <%: Html.Label("Subject:")%>
            <%: Model.EmailQueue.Subject%>
        </li>
        <li>
            <%: Html.Label("Body:")%>
            <%: Html.HtmlEncode(Model.EmailQueue.Body)%>
        </li>
        </ul>
    </fieldset>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

