<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.InvestigatorViewModel>" %>
        <ul>
            <li>

                <%: Html.CheckBoxFor(model => model.Investigator.IsPrimary)%> <%: Html.Encode("Primary Investigator") %>
                <%: Html.ValidationMessageFor(model => model.Investigator.IsPrimary)%>

            </li>  
            <li> 

                <%: Html.LabelFor(model => model.Investigator.Name)%>

                <%: Html.TextBoxFor(model => model.Investigator.Name)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Name)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Position)%>

                <%: Html.TextBoxFor(model => model.Investigator.Position)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Position)%>
 
            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Institution)%>


                <%: Html.TextBoxFor(model => model.Investigator.Institution)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Institution)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Address1)%>

                <%: Html.TextBoxFor(model => model.Investigator.Address1)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address1)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Address2)%>

                <%: Html.TextBoxFor(model => model.Investigator.Address2)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address2)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Address3)%>

                <%: Html.TextBoxFor(model => model.Investigator.Address3)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address3)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.City)%>


                <%: Html.TextBoxFor(model => model.Investigator.City)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.City)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.State)%>

                <%: Html.TextBoxFor(model => model.Investigator.State)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.State)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Zip)%>

                <%: Html.TextBoxFor(model => model.Investigator.Zip)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Zip)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Phone)%>

                <%: Html.TextBoxFor(model => model.Investigator.Phone)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Phone)%>

            </li>
            <li>

                <%: Html.LabelFor(model => model.Investigator.Email)%>

                <%: Html.TextBoxFor(model => model.Investigator.Email)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Email)%>

            </li>
<%--            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Notes)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Notes)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Notes)%>
            </div>--%>

            </ul>