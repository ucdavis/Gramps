<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallForProposalListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Calls Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Calls Index</h2><br />


<p>
    <%: Html.ActionLink<CallForProposalController>(a => a.Create(), "Create Call", new {@class="button"}) %>
</p>

<% Html.Grid(Model.CallForProposals) 
            .Name("List")
            .PrefixUrlParameters(true) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%--<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>--%> 
                <%: Html.ActionLink<CallForProposalController>(a => a.Edit(x.Id), " ", new { @class = "edit_button" })%>          
				<%}).Title("Edit");
            col.Template(x => {%>
                <%: Html.ActionLink<CallForProposalController>(a => a.Launch(x.Id), " ", new { @class = "launch_button" })%>          
				<%}).Title("Launch");
			            col.Bound(x => x.Name);
                        col.Bound(x => x.IsActive);
                        col.Bound(x => x.CreatedDate);
                        col.Bound(x => x.EndDate);
                        col.Bound(x => x.CallsSentDate);
                        })
            .Pageable()
            .Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <style type="text/css">
        .launch_button
        {
            background: url("../Images/Rocket_launch.png") no-repeat scroll 0 0 transparent;
            border:0;         
            display: inline-block;
            height: 22px;
            width: 22px;
        }
        .launch_button:hover
        {
            color:#0D548A;
        }
    </style>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

