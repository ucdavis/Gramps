<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallReportViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("CallNavigationButtons"); %>
    <%: Html.Hidden("callForProposalId", Model.CallForProposal.Id) %>

    <h2>Create Report</h2>

    <% Html.RenderPartial("ReportFormForCall"); %>


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

            //If a server side error happened, this reloads the selected report columns.
            <%if(Model.Report != null){
                foreach (var irc in Model.Report.ReportColumns){
                    var columnId = irc.IsProperty
                                       ? "property" + irc.Name
                                       : Model.Questions.Where(a => a.Name == irc.Name).FirstOrDefault().Id.ToString() ;%>
                    var spans = $('#<%=columnId%>');
                    $.each(spans, function(index2, item2) {
                        $(item2).find("div.button").click();
                    });                     
                <%}%>                
            <%}%>
        });

        function CreateRow(span, button) {
            if ($(button).hasClass("selected")) {
                var tbody = $("div#selectedColumns").find("tbody");

                var tr = $("<tr>").addClass("dataRow").attr("id", $(span).attr("id"));

                var cell1 = $("<td>");
                cell1.append($(span).find("input.indexedControl[type='hidden']").clone());

                tr.append(cell1);
                tr.append($("<td>").html($(span).find("label.indexedControl").html()));
//                tr.append($("<td>").html($("<input>").attr("type", "textbox").attr("id", "_Format").attr("name", "_Format")));

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
        #toggle_all:hover 
        {
        	color: blue; 
        	cursor: pointer;
        }
    </style>



</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>
