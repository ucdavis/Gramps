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

    <div id="filter_container">
        <h3><a href="#">Filters</a></h3>
        <% using (Html.BeginForm("Index", "CallForProposal", FormMethod.Post)) { %>
            <%= Html.AntiForgeryToken() %>
            <ul>
            <li>
            <span id = "IsActiveSpan">
            <label for="Approved"></label>
                <input type="radio" id="Active" name="filterActive" value="Active" "<%=Model.FilterActive == "Active" ? "checked" : string.Empty%>" /><label for="active">Active</label>
                <input type="radio" id="InActive" name="filterActive" value="InActive" "<%= Model.FilterActive == "InActive" ? "checked" : string.Empty %>" /><label for="inactive">InActive</label>
                <input type="radio" id="Both" name="filterActive" value="Both" "<%= string.IsNullOrWhiteSpace(Model.FilterActive) || Model.FilterActive == "Both" ? "checked" : string.Empty %>" /><label for="both">Both</label>
            </span>
            </li>
            <li>
                <%: Html.Label("Create Date is After:") %>
                <%: Html.TextBoxFor(a => a.FilterStartCreate) %>
            </li>
            <li>
                <%: Html.Label("Create Date is Before:") %>
                <%: Html.TextBoxFor(a => a.FilterEndCreate) %>
            </li>

            <li><strong></strong><%= Html.SubmitButton("Submit", "Filter") %></li>
        </ul>
        <% } %>
    </div>

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
            .DataBinding(binding => binding.Server().Select<CallForProposalController>(a => a.Index(Model.FilterActive, Model.FilterStartCreate, Model.FilterEndCreate)))
            .Pageable()
            .Sortable(s => s.OrderBy(a => a.Add(b => b.CreatedDate).Descending()))
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
     <script type="text/javascript">
         $(function () {
             $("#filter_container").accordion({ collapsible: true, autoHeight: false, active: false });
         });
    </script>
 
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

    <script type="text/javascript">
        $(document).ready(function () {
            $("#FilterStartCreate").datepicker();
            $("#FilterEndCreate").datepicker();           
        });
    </script>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

