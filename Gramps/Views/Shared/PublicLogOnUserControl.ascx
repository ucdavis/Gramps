<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%= Html.Encode(Page.User.Identity.Name) %></b>!   
        
         <%= Html.ActionLink<PublicController>(a => a.LogOff(), "Log Off")%>  
         <%= Html.ActionLink<HomeController>(a => a.Index(), "Home - Change") %>  
         <%= Html.ActionLink<HomeController>(a => a.About(), "About") %>          
<%
    }
    else {
%> 
        <%--[ <%= Html.ActionLink("Log On", "LogOn", "Account")%> ]--%>
        <%--[ <%= Html.ActionLink<AccountController>(a => a.LogOn(ReturnUrlGenerator.LogOnReturn(Model != null && Model.AppName != null ? Model.AppName : string.Empty)), "Log On")%> ]--%>
        <%= Html.ActionLink<PublicController>(a => a.LogOn(), "Log On")%>
        <%= Html.ActionLink<HomeController>(a => a.Index(), "Home - Change")%>  
        <%= Html.ActionLink<HomeController>(a => a.About(), "About") %>
<%
    }
%>