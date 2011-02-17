<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailQueueViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Edit</h2>

	<%= Html.ClientSideValidation<EmailQueue>()%>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.HiddenFor(a => a.EmailQueue.Id) %>
        <%: Html.HiddenFor(a => a.CallForProposal.Id) %>
        <%: Html.ValidationSummary() %>

        <ul class="registration_form" id="left_bar">
        <li>
            <strong>Pending</strong>
            <%= Html.CheckBoxFor(a => a.EmailQueue.Pending) %>
            <%= Html.ValidationMessageFor(a => a.EmailQueue.Pending)%> 
        </li>
        <li>
            <strong>Immediate</strong>
            <%= Html.CheckBoxFor(a => a.EmailQueue.Immediate) %>
        </li>

        <li>
            <strong>Subject: </strong>
            <%: Html.TextBox("EmailQueue.Subject", Model.EmailQueue != null ? Model.EmailQueue.Subject : string.Empty, new { @style = "width:20em;" })%>
        </li>
        <li>
            <strong>BodyText:</strong>
            <%= Html.TextArea("EmailQueue.Body", Model.EmailQueue != null ? Model.EmailQueue.Body : string.Empty)%>
            <%= Html.ValidationMessageFor(a => a.EmailQueue.Body)%> 
        </li>
        <li>
            <input type="submit" value="Save" />
        </li>
        </ul>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $("#EmailQueue_Body").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "700" });

        });

   </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

