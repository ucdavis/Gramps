using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;


namespace Gramps.Controllers.ViewModels
{
    public class ReportLaunchViewModel : CallNavigationViewModel
    {
        public ReportLaunchViewModel()
        {
            ColumnNames = new List<string>();
            RowValues = new List<string[]>();
            AvailableQuestions = new List<string>();
        }

        public ICollection<string> ColumnNames { get; set; }
        public ICollection<string[]> RowValues { get; set; }
        public Report Report { get; set; }
        public List<string> AvailableQuestions { get; set; }

        public static ReportLaunchViewModel Create(IRepository repository, CallForProposal callForProposal, Report report)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new ReportLaunchViewModel { CallForProposal = callForProposal, Report = report};
            viewModel.AvailableQuestions = callForProposal.Questions.Select(a => a.Name).ToList();

            return GenerateGeneric(viewModel, report, callForProposal);
        }

        private static ReportLaunchViewModel GenerateGeneric(ReportLaunchViewModel viewModel, Report report, CallForProposal callForProposal)
        {
            //deal with the column names
            foreach (var reportColumn in report.ReportColumns)
            {
                if (reportColumn.IsProperty || viewModel.AvailableQuestions.Contains(reportColumn.Name))
                {
                    viewModel.ColumnNames.Add(reportColumn.Name);
                }
            }

            foreach (var proposal in callForProposal.Proposals)
            {
                var row = new List<string>();

                foreach (var reportColumn in report.ReportColumns)
                {
                    if (reportColumn.IsProperty || viewModel.AvailableQuestions.Contains(reportColumn.Name))
                    {
                        row.Add(ExtractValue(reportColumn, proposal));
                    }
                }

                viewModel.RowValues.Add(row.ToArray());
            }

            return viewModel;
        }

        private static string ExtractValue(ReportColumn reportColumn, Proposal proposal)
        {
            var result = string.Empty;

            if (!reportColumn.IsProperty)
            {
                var answer = proposal.Answers.Where(a => a.Question.Name == reportColumn.Name).FirstOrDefault();

                if (answer != null)
                {
                    result = answer.Answer;
                }
            }
            else
            {
                if (reportColumn.Name == StaticValues.Report_Submitted)
                {
                    result = proposal.IsSubmitted.ToString();
                }
                else if (reportColumn.Name == StaticValues.Report_Approved)
                {
                    result = proposal.IsApproved.ToString();
                }
                else if(reportColumn.Name == StaticValues.Report_Sequence)
                {
                    result = proposal.Sequence.ToString();
                }
            }

            return result;
        }
    }
}