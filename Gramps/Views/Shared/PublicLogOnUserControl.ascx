<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%= Html.Encode(Page.User.Identity.Name) %></b>!   
        
         <%= Html.ActionLink<PublicController>(a => a.LogOff(), "Log Off")%>  
         <%= Html.ActionLink<ProposalController>(a => a.Home(), "Home")%>          
<%
    }
    else {
%> 
        <%= Html.ActionLink<PublicController>(a => a.LogOn(), "Log On")%>
        <%= Html.ActionLink<ProposalController>(a => a.Home(), "Home")%>  
<%
    }
%>