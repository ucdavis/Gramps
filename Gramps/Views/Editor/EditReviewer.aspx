<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EditorViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	EditReviewer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Edit Reviewer</h2>

	<%= Html.ClientSideValidation<Gramps.Controllers.ViewModels.EditorViewModel>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Fields</legend>
            <ul>
            <li>
                <%: Html.LabelFor(model => model.Editor.ReviewerEmail) %>
                <%if(Model.IsCallForProposal) {%>
                    <%: Html.DisplayFor(model => model.Editor.ReviewerEmail)%>
                <%}%>
                <%else{%>
                    <%: Html.TextBoxFor(model => model.Editor.ReviewerEmail)%>
                    <%: Html.ValidationMessageFor(model => model.Editor.ReviewerEmail)%>
                <%} %>
            </li>
            
            <li>
                <%: Html.LabelFor(model => model.Editor.ReviewerName)%>
                <%: Html.TextBoxFor(model => model.Editor.ReviewerName)%>
                <%: Html.ValidationMessageFor(model => model.Editor.ReviewerName)%>
            </li>
            <%if(Model.IsCallForProposal) {%>
                <li>
                    <%: Html.LabelFor(model => model.Editor.HasBeenNotified)%>
                    <%: Html.CheckBoxFor(model => model.Editor.HasBeenNotified)%>
                    <%: Html.ValidationMessageFor(model => model.Editor.HasBeenNotified)%>
                </li>   
            <%}%>
            <p>
                <input type="submit" value="Save" />
            </p>
            </ul>
        </fieldset>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

