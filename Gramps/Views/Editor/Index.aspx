<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EditorListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%= Html.ActionLink<TemplateController>(a => a.Index() , "Back to List") %>
        </li>
        <li>
            <%if (Model.isTemplate){%>
            <%:Html.ActionLink<TemplateController>(a => a.Edit((int)Model.templateId), "Details")%>
            <%}else{%>
            <%--<%:Html.ActionLink<TemplateController>(a => a.Edit((int) Model.templateId), "Details")%>--%>
            <%}%>
        </li>
        <li>
            <%: Html.ActionLink<EditorController>(a => a.Index(Model.templateId, Model.callForProposalId), "Editors/Reviewers")%>
        </li>

    </ul>

    <h2>Index</h2>

<p>
    <%: Html.ActionLink("Create New", "Create") %>
</p>

<% Html.Grid(Model.editorList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>           
				<%});
			col.Template(x => {%>
				<%: Html.ActionLink("Details", "Details", new { id = x.Id }) %>           
				<%});
			            col.Bound(x => x.IsOwner);
                        col.Bound(x => x.ReviewerEmail);
                        col.Bound(x => x.ReviewerName);
                        col.Bound(x => x.ReviewerId);
                        col.Template(x => {%>
				<%: Html.ActionLink("Delete", "Delete", new { id = x.Id }) %>           
				<%});
            })
            //.Pageable()
            //.Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

