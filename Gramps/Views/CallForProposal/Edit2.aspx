<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Core.Domain.CallForProposal>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit2
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit2</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name) %>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.IsActive) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.IsActive) %>
                <%: Html.ValidationMessageFor(model => model.IsActive) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CreatedDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CreatedDate, String.Format("{0:g}", Model.CreatedDate)) %>
                <%: Html.ValidationMessageFor(model => model.CreatedDate) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.EndDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.EndDate, String.Format("{0:g}", Model.EndDate)) %>
                <%: Html.ValidationMessageFor(model => model.EndDate) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CallsSentDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CallsSentDate, String.Format("{0:g}", Model.CallsSentDate)) %>
                <%: Html.ValidationMessageFor(model => model.CallsSentDate) %>
            </div>
            

            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Id) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Id) %>
                <%: Html.ValidationMessageFor(model => model.Id) %>
            </div>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

