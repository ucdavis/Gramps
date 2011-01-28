<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EditorListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editor Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%= Html.ActionLink<TemplateController>(a => a.Index() , "Back to List") %>
        </li>
        <li>
            <%if (Model.IsTemplate){%>
            <%:Html.ActionLink<TemplateController>(a => a.Edit((int)Model.TemplateId), "Details")%>
            <%}else{%>
            <%--<%:Html.ActionLink<TemplateController>(a => a.Edit((int) Model.templateId), "Details")%>--%>
            <%}%>
        </li>
        <li>
            <%: Html.ActionLink<EditorController>(a => a.Index(Model.TemplateId, Model.CallForProposalId), "Editors/Reviewers")%>
        </li>

    </ul>

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
				    <%:Html.ActionLink("Edit", "Edit", new {id = x.Id})%>     
                <%}%>      
				<%});
			col.Template(x => {%>
				<%: Html.ActionLink("Details", "Details", new { id = x.Id }) %>           
				<%});
			            col.Bound(x => x.IsOwner);
                        col.Bound(x => x.ReviewerName).Title("Name");
                        col.Bound(x => x.ReviewerEmail);                        
                        col.Bound(x => x.ReviewerId);
                        col.Template(x => { %>  
                            <% if (x.IsOwner == false){%>                                      
                            <% using (Html.BeginForm("Delete", "Editor", FormMethod.Post)) { %>
                                <%= Html.AntiForgeryToken() %>
                                <%: Html.Hidden("id", x.Id) %>
                                <%: Html.Hidden("TemplateId", Model.TemplateId) %>
                                <%: Html.Hidden("CallForProposalId", Model.CallForProposalId) %>
                                <%= Html.SubmitButton("Submit", "Remove", new {@class="remove_button"}) %>
                                                                           
                            <% } %>  
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

