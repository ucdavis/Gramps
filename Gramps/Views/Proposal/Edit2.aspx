<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Core.Domain.Proposal>" %>

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
                <%: Html.LabelFor(model => model.Guid) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Guid) %>
                <%: Html.ValidationMessageFor(model => model.Guid) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Email) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Email) %>
                <%: Html.ValidationMessageFor(model => model.Email) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.IsApproved) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.IsApproved) %>
                <%: Html.ValidationMessageFor(model => model.IsApproved) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.IsDenied) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.IsDenied) %>
                <%: Html.ValidationMessageFor(model => model.IsDenied) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.IsNotified) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.IsNotified) %>
                <%: Html.ValidationMessageFor(model => model.IsNotified) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.IsSubmitted) %>
            </div>
            <div class="editor-field">
                <%: Html.CheckBoxFor(model => model.IsSubmitted) %>
                <%: Html.ValidationMessageFor(model => model.IsSubmitted) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.RequestedAmount) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.RequestedAmount, String.Format("{0:F}", Model.RequestedAmount)) %>
                <%: Html.ValidationMessageFor(model => model.RequestedAmount) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.ApprovedAmount) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.ApprovedAmount, String.Format("{0:F}", Model.ApprovedAmount)) %>
                <%: Html.ValidationMessageFor(model => model.ApprovedAmount) %>
            </div>
            

            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.NotifiedDate) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.NotifiedDate, String.Format("{0:g}", Model.NotifiedDate)) %>
                <%: Html.ValidationMessageFor(model => model.NotifiedDate) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.WasWarned) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.WasWarned) %>
                <%: Html.ValidationMessageFor(model => model.WasWarned) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Sequence) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Sequence) %>
                <%: Html.ValidationMessageFor(model => model.Sequence) %>
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

