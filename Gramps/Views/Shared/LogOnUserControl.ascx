﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%= Html.Encode(Page.User.Identity.Name) %></b>!   
        
        [ <%= Html.ActionLink("Log Off", "LogOut", "Account")%> ]           
<%
    }
    else {
%> 
        <%--[ <%= Html.ActionLink("Log On", "LogOn", "Account")%> ]--%>
        <%--[ <%= Html.ActionLink<AccountController>(a => a.LogOn(ReturnUrlGenerator.LogOnReturn(Model != null && Model.AppName != null ? Model.AppName : string.Empty)), "Log On")%> ]--%>
        [ <%= Html.ActionLink<AccountController>(a => a.LogOn(string.Empty), "Log On")%> ]
<%
    }
%>