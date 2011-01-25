<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.TemplateViewModel>" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>
    <div id="tabs">
        <ul>
            <li><a href="#<%= StaticValues.Tab_Details %>">Details</a></li>
            <li><a href="#<%= StaticValues.Tab_Editors %>">Editors</a></li>
            <li><a href="#<%= StaticValues.Tab_Questions %>">Questions</a></li>
            <li><a href="#<%= StaticValues.Tab_Emails %>">Email List</a></li>
            <li><a href="#<%= StaticValues.Tab_EmailTemplates %>">Email Templates</a></li>
        </ul>
        <div id="<%= StaticValues.Tab_Details %>">
        
<%--            <% using (Html.BeginForm("Edit", "ItemManagement", FormMethod.Post, new { @enctype = "multipart/form-data" }))
               {%>

            <%
                Html.RenderPartial(StaticValues.Partial_ItemForm);%>

            <% }%>--%>
        
        </div>
        <div id="<%= StaticValues.Tab_Editors %>">
        </div>
        <div id="<%= StaticValues.Tab_Questions %>">
        </div>
        <div id="<%= StaticValues.Tab_Emails %>">
        </div>
        <div id="<%= StaticValues.Tab_EmailTemplates %>">
        </div>
    </div>

	<%= Html.ClientSideValidation<Gramps.Controllers.TemplateViewModel>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>




        <fieldset>
            <legend>Fields</legend>
            
            <p>
                <input type="submit" value="Edit" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

