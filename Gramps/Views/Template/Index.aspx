<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Gramps.Core.Domain.Template>>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Template Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Template Index</h2><br/>


<p>
    <%: Html.ActionLink<TemplateController>(a => a.Create(), "Create Template", new {@class="button"}) %>
</p>

<% Html.Grid(Model) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%--<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>--%>  
                <%: Html.ActionLink<TemplateController>(a => a.Edit(x.Id), ".   .", new { @class = "edit_button" }) %>         
				<%}).Title("Edit");
			            col.Bound(x => x.Name);
                        col.Bound(x => x.IsActive);
                        })
            .Pageable()
            .Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

