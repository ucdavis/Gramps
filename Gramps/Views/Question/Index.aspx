<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.QuestionListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Question Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Index</h2><br/>


<p>
    <%: Html.ActionLink<QuestionController>(a => a.Create(Model.TemplateId, Model.CallForProposalId), "Create", new {@class="button"}) %>
</p>

<% Html.Grid(Model.QuestionList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				    <%:Html.ActionLink<QuestionController>(a => a.Edit(x.Id, Model.TemplateId, Model.CallForProposalId), "Edit")%>           
				<%});
			col.Template(x => {%>
				<%: Html.ActionLink("Details", "Details", new { id = x.Id }) %>           
				<%});
                                col.Bound(x => x.Order);
                                col.Bound(x => x.Name);
                                col.Bound(x => x.QuestionType.Name).Title("Type");
                                col.Bound(x => x.OptionChoices).Title("Options");
                                col.Bound(x => x.ValidationClasses).Title("Validators");
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

