<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallForProposalCreateViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Call
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Pick Template To Create Call From</h2>

<% Html.Grid(Model.Templates) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%--<%: Html.ActionLink<CallForProposalController>(a => a.Create(x.Id), "Create Call")%>   --%> 
				    <% using (Html.BeginForm("Create", "CallForProposal", FormMethod.Post)){ %>
                    <%= Html.AntiForgeryToken() %>
                    <%: Html.Hidden("templateId", x.Id) %>
                    <%= Html.SubmitButton("Submit", "Create Call") %>                                                                           
                <%}%>        
				<%});
			            col.Bound(x => x.Name);
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

