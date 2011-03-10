<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ReportViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Create Report</h2>

    <%= Html.ClientSideValidation<Report>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

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
                            <td class="t-header">Format</td>
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

        <fieldset class="indexedControlContainer">
        <legend>Properties</legend>
        <br />
            <span id="propertySubmitted" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Submitted")%></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_Submitted, new { @class = StaticValues.Class_indexedControl })%>                
            </span> 
            <span id="propertyApproved" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Approved")%></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", "Approved", new { @class = StaticValues.Class_indexedControl })%>                
            </span> 
        </fieldset>


        <p>
            <input type="submit" value="Create" />
        </p>
    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/jquery.CaesMutioptionControl.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/RenameForArray.js") %>" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            // change the look of the checkboxes
            $("input[type='CheckBox']").CaesMutioptionControl({ width: '900px' });

            $("div.button").live("click", function () {
                CreateRow($(this).parent(), this);

                RenameControls($("div#selectedColumns"), "createReportParameters", "tr.dataRow");
            });

            $("#toggle_all").click(function () {
                var containers = $(".indexedControlContainer");
                $.each(containers, function (index, item) {
                    var spans = $(item).find("span");
                    $.each(spans, function (index2, item2) {
                        $(item2).find("div.button").click();
                    });
                });
            });
        });

        function CreateRow(span, button) {
            if ($(button).hasClass("selected")) {
                var tbody = $("div#selectedColumns").find("tbody");

                // this only works against properties with real question id, not property
                var tr = $("<tr>").addClass("dataRow").attr("id", $(span).attr("id")); //.find("input#_QuestionId").val());

                var cell1 = $("<td>");
                cell1.append($(span).find("input.indexedControl[type='hidden']").clone());

                tr.append(cell1);
                tr.append($("<td>").html($(span).find("label.indexedControl").html()));
                tr.append($("<td>").html($("<input>").attr("type", "textbox").attr("id", "_Format").attr("name", "_Format")));

                tbody.append(tr);
            }
            else {
                // deal with properties
                if ($(span).hasClass("property")) {
                    $("tr#" + $(span).attr("id")).remove();
                }
                else {
                    $("tr#" + $(span).find("input#_QuestionId").val()).remove();
                }
            }
        }

    </script>

    <style type="text/css">
        .button.selected {background:LightYellow}
        .button:hover 
        {
        	color: blue; 
        	cursor: pointer;
        	border: transparent;
        }
        .button
        {
        	text-align: left;
        	border: transparent;
        }
    </style>



</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>
