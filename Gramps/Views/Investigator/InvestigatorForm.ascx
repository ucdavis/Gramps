<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.InvestigatorViewModel>" %>

            <div class="editor-field">
                <%: Html.CheckBoxFor(model => model.Investigator.IsPrimary)%> <%: Html.Encode("Primary Investigator") %>
                <%: Html.ValidationMessageFor(model => model.Investigator.IsPrimary)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Name)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Name)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Institution)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Institution)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Institution)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Address1)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Address1)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address1)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Address2)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Address2)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address2)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Address3)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Address3)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address3)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.City)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.City)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.City)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.State)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.State)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.State)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Zip)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Zip)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Zip)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Phone)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Phone)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Phone)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Email)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Email)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Email)%>
            </div>
            
<%--            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Notes)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Notes)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Notes)%>
            </div>--%>