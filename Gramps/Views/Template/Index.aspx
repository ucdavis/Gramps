<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Gramps.Core.Domain.Template>>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>


<p>
    <%: Html.ActionLink("Create New", "Create") %>
</p>

<% Html.Grid(Model) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>           
				<%});
			            col.Bound(x => x.Name);
                        col.Bound(x => x.IsActive);
                        })
            //.Pageable()
            //.Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

