<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailsForCallListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>List of emails to send the call for proposal to</h2><br/>


<p>
    <%: Html.ActionLink<EmailsForCallController>(a => a.Create(Model.TemplateId, Model.CallForProposalId), "Add Email", new {@class="button"}) %>
    <%: Html.ActionLink<EmailsForCallController>(a => a.BulkCreate(Model.TemplateId, Model.CallForProposalId), "Bulk Add Emails", new {@class="button"}) %>
</p>

<% Html.Grid(Model.EmailsForCallList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
             col.Template(x => {%>
				    <%:Html.ActionLink<EmailsForCallController>(a => a.Edit(x.Id, Model.TemplateId, Model.CallForProposalId), ".   .", new { @class = "edit_button" })%>           
				<%}).Title("Edit");

            col.Bound(x => x.Email);
            if(!Model.IsTemplate){
            col.Bound(x => x.HasBeenEmailed);
            col.Bound(x => x.EmailedOnDate);
            }
            col.Template(x => { %>                                                       
                <% using (Html.BeginForm("Delete", "EmailsForCall", FormMethod.Post)) { %>
                    <%= Html.AntiForgeryToken() %>
                    <%: Html.Hidden("id", x.Id) %>
                    <%: Html.Hidden("TemplateId", Model.TemplateId) %>
                    <%: Html.Hidden("CallForProposalId", Model.CallForProposalId) %>
                    <%= Html.SubmitButton("Submit", "Remove", new {@class="remove_button"}) %>
                                                                            
                <%}%>                                       
            <% });
            })
            .Pageable()
            .Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

