using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Gramps.Services
{
    public interface IReportService
    {
        Report CommonCreate(ModelStateDictionary modelState, Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters, string showSubmitted);
    }

    public class ReportService: IReportService
    {
        private readonly IRepository _repository;

        public ReportService(IRepository repository)
        {
            _repository = repository;
        }

        public Report CommonCreate(ModelStateDictionary modelState, Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters, string showSubmitted)
        {
            Template template = null;
            CallForProposal callforProposal = null;
            var availableQuestionDict = new Dictionary<int, string>();
            if (templateId.HasValue && templateId.Value != 0)
            {
                template = _repository.OfType<Template>().GetNullableById(templateId.Value);
                availableQuestionDict = template.Questions.ToDictionary(question => question.Id, question => question.Name);
            }
            else if (callForProposalId.HasValue && callForProposalId.Value != 0)
            {
                callforProposal = _repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                availableQuestionDict = callforProposal.Questions.ToDictionary(question => question.Id, question => question.Name);
            }

            if (createReportParameters == null)
            {
                createReportParameters = new CreateReportParameter[0];
            }


            var reportToCreate = new Report();
            reportToCreate.Template = template;
            reportToCreate.CallForProposal = callforProposal;
            reportToCreate.Name = report.Name;
            reportToCreate.ShowUnsubmitted = showSubmitted == "ShowAll" ? true : false;

            var count = 0;
            foreach (var createReportParameter in createReportParameters)
            {
                var reportColumn = new ReportColumn();
                reportColumn.ColumnOrder = count++;
                if (createReportParameter.Property)
                {
                    reportColumn.IsProperty = createReportParameter.Property;
                    reportColumn.Name = createReportParameter.PropertyName;
                }
                else
                {
                    if (availableQuestionDict.ContainsKey(createReportParameter.QuestionId))
                    {
                        reportColumn.Name = availableQuestionDict[createReportParameter.QuestionId];
                    }
                }
                reportToCreate.AddReportColumn(reportColumn);
            }

            if (reportToCreate.ReportColumns.Count == 0)
            {
                modelState.AddModelError("ReportColumns", "Must select at least one column to report on");
            }


            reportToCreate.TransferValidationMessagesTo(modelState);

            return reportToCreate;
        }
    }
}