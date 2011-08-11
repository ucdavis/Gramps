using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Core.Domain;
using Rhino.Mocks;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Gramps.Tests.Core.Helpers
{
    public abstract class ControllerRecordFakes<T> where T : DomainObject
    {
        public void Records(int count, IRepository<T> repository)
        {
            var records = new List<T>();
            Records(count, repository, records);
        }


        public void Records(int count, IRepository<T> repository, List<T> specificRecords)
        {
            var records = new List<T>();
            var specificRecordsCount = 0;
            if (specificRecords != null)
            {
                specificRecordsCount = specificRecords.Count;
                for (int i = 0; i < specificRecordsCount; i++)
                {
                    records.Add(specificRecords[i]);
                }
            }

            for (int i = 0; i < count; i++)
            {
                records.Add(CreateValid(i + specificRecordsCount + 1));
            }

            var totalCount = records.Count;
            for (int i = 0; i < totalCount; i++)
            {
                records[i].SetIdTo(i + 1);
                int i1 = i;
                repository
                    .Expect(a => a.GetNullableById(i1 + 1))
                    .Return(records[i])
                    .Repeat
                    .Any();
            }
            repository.Expect(a => a.GetNullableById(totalCount + 1)).Return(null).Repeat.Any();
            repository.Expect(a => a.Queryable).Return(records.AsQueryable()).Repeat.Any();
            repository.Expect(a => a.GetAll()).Return(records).Repeat.Any();
        }

        protected abstract T CreateValid(int i);
    }

    public class FakeEditors : ControllerRecordFakes<Editor>
    {
        protected override Editor CreateValid(int i)
        {
            return CreateValidEntities.Editor(i);
        }
    }

    public class FakeCallForProposals : ControllerRecordFakes<CallForProposal>
    {
        protected override CallForProposal CreateValid(int i)
        {
            return CreateValidEntities.CallForProposal(i);
        }
    }

    public class FakeUsers : ControllerRecordFakes<User>
    {
        protected override User CreateValid(int i)
        {
            return CreateValidEntities.User(i);
        }
    }

    public class FakeTemplates : ControllerRecordFakes<Template>
    {
        protected override Template CreateValid(int i)
        {
            return CreateValidEntities.Template(i);
        }
    }

    public class FakeEmailQueues : ControllerRecordFakes<EmailQueue>
    {
        protected override EmailQueue CreateValid(int i)
        {
            return CreateValidEntities.EmailQueue(i);
        }
    }

    public class FakeEmailsForCall : ControllerRecordFakes<EmailsForCall>
    {
        protected override EmailsForCall CreateValid(int i)
        {
            return CreateValidEntities.EmailsForCall(i);
        } 
    }

    public class FakeEmailTemplates : ControllerRecordFakes<EmailTemplate>
    {
        protected override EmailTemplate CreateValid(int i)
        {
            return CreateValidEntities.EmailTemplate(i);
        }
    }

    public class FakeProposals : ControllerRecordFakes<Proposal>
    {
        protected override Proposal CreateValid(int i)
        {
            return CreateValidEntities.Proposal(i);
        }
    }

    public class FakeInvestigators : ControllerRecordFakes<Investigator>
    {
        protected override Investigator CreateValid(int i)
        {
            return CreateValidEntities.Investigator(i);
        }
    }

    public class FakeReviewedProposals : ControllerRecordFakes<ReviewedProposal>
    {
        protected override ReviewedProposal CreateValid(int i)
        {
            return CreateValidEntities.ReviewedProposal(i);
        }
    }

    public class FakeComments : ControllerRecordFakes<Comment>
    {
        protected override Comment CreateValid(int i)
        {
            return CreateValidEntities.Comment(i);
        }
    }

    public class FakeQuestions : ControllerRecordFakes<Question>
    {
        protected override Question CreateValid(int i)
        {
            return CreateValidEntities.Question(i);
        }
    }
}
