﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.CallReportViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

    <%= Html.ClientSideValidation<Report>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("Please correct errors") %>

        <fieldset>
            <legend>Report</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Report.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Report.Name)%>
                <%: Html.ValidationMessageFor(model => model.Report.Name)%>
            </div>           
        </fieldset>

        <div id="toggle_all">
            Toggle Selected Columns
        </div>

        <fieldset>
            <legend>Selected Columns</legend>           
            <div id="selectedColumns" class="t-widget t-grid">
                <table cellspacing=0>
                    <thead>
                        <tr>
                            <td class="t-header"></td>
                            <td class="t-header">Field Name</td>
                            <%--<td class="t-header">Format</td>--%>
                        </tr>
                    </thead>
                    <tbody>

                    </tbody>
                </table>
            </div>
        </fieldset>

        <fieldset class="indexedControlContainer">
            <legend>Questions</legend>
            <br />
            <% foreach (var question in Model.Questions){%>
                <span id='<%= question.Id %>'>
                    <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                    <label for="Selected" class="indexedControl"><%= Html.Encode(question.Name)%></label>                    
                    <input id="_QuestionId" class="indexedControl" type="hidden" value="<%= question.Id %>" name="_QuestionId"/>                 
                </span>
            <%}%>
        </fieldset>

        <% Html.RenderPartial("ReportPropertyColumns"); %>


        <p>
            <input type="submit" value="Save" />
        </p>
    <% } %>
