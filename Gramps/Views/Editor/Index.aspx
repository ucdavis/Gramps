<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EditorListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editor Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Index</h2><br/>

<p>
    <%: Html.ActionLink<EditorController>(a => a.CreateReviewer(Model.TemplateId, Model.CallForProposalId), "Create Reviewer", new {@class="button"}) %>
    <%: Html.ActionLink<EditorController>(a => a.AddEditor(Model.TemplateId, Model.CallForProposalId), "Create Editor", new {@class="button"}) %>
</p>

<% Html.Grid(Model.EditorList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .CellAction(cell =>
                            {
                                if (cell.Column.Member == "ReviewerId")
                                {
                                    cell.Text = cell.DataItem.User != null ? " " : cell.DataItem.ReviewerId.ToString();
                                }
                                if (cell.Column.Member == "ReviewerName")
                                {
                                    cell.Text = cell.DataItem.User != null
                                                    ? cell.DataItem.User.FullName
                                                    : cell.DataItem.ReviewerName;
                                }
                            })
            .Columns(col => {
            col.Template(x => {%>
                <% if (x.User == null){%>
				    <%:Html.ActionLink<EditorController>(a => a.EditReviewer(x.Id, Model.TemplateId, Model.CallForProposalId), " ", new { @class = "edit_button" })%>     
                <%}%>      
				<%}).Title("Edit");
			col.Template(x => {%>
                <% if (x.User == null){%>
				    <% using (Html.BeginForm("ResetReviewerId", "Editor", FormMethod.Post)){ %>
                    <%= Html.AntiForgeryToken() %>
                    <%: Html.Hidden("id", x.Id) %>
                    <%: Html.Hidden("TemplateId", Model.TemplateId) %>
                    <%: Html.Hidden("CallForProposalId", Model.CallForProposalId) %>
                    <%= Html.SubmitButton("Submit", "Reset") %>                                                                           
                    <% } %> 
                <%}%>        
				<%});
			            col.Bound(x => x.IsOwner);
                        col.Bound(x => x.ReviewerName).Title("Name");
                        col.Bound(x => x.ReviewerEmail);                        
                        col.Bound(x => x.ReviewerId);
                        if (Model.IsCallForProposal)
                        {
                            col.Bound(x => x.HasBeenNotified).Title("Notified");
                            col.Bound(x => x.NotifiedDate);
                        }
                        col.Template(x => { %>  
                            <% if (x.IsOwner == false){%>                                      
                            <% using (Html.BeginForm("Delete", "Editor", FormMethod.Post)) { %>
                                <%= Html.AntiForgeryToken() %>
                                <%: Html.Hidden("id", x.Id) %>
                                <%: Html.Hidden("TemplateId", Model.TemplateId) %>
                                <%: Html.Hidden("CallForProposalId", Model.CallForProposalId) %>
                                <%= Html.SubmitButton("Submit", " ", new {@class="remove_button"}) %>
                                                                           
                            <% } %>  
                            <%}%>                                       
                        <% }).Title("Delete");
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

