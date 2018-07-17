using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using TheFlow.CoreConcepts;

namespace TheFlow.Notifications.WebApi.Features.Queries
{
    [Route("api/v1/[controller]")]
    public class QueriesController : Controller
    {
        private readonly ProcessManager _manager;
        private readonly IDocumentStore _store;

        public QueriesController(
            ProcessManager manager,
            IDocumentStore store
        )
        {
            _manager = manager;
            _store = store;
        }

        [HttpGet]
        [Route("Process/{id}")]
        public ProcessInstance GetProcessInstance(
            Guid id
        )
        {
            return _manager.InstancesStore.GetById(id);
        }

        [HttpGet]
        [Route("UnprocessedMessagesCount/{userId}")]
        public int GetUnprocessedMessagesCount(string userId)
        {
            using (var session = _store.OpenSession())
            {
                return session.Advanced.RawQuery<ProcessInstance>(
                    @"from index 'Notifications/Unprocessed'
                      where IsRunning = true and To = '" + userId + "'"
                ).Count();
            }
        }

        [HttpGet]
        [Route("UnprocessedMessages/{userId}")]
        public IEnumerable<MessageViewModel> GetUnprocessedMessages(string userId)
        {
            using (var session = _store.OpenSession())
            {
                return session.Advanced.RawQuery<MessageViewModel>(
                    @"from index 'Notifications/Unprocessed' as n
                      where IsRunning = true and To = '" + userId + @"' 
                      select {
                        From: n.History[0].Payload.From,
                        When: n.History[0].When,
                        Message: n.History[0].Payload.Message,
                        ActionUrl: n.History[0].Payload.ActionUrl
                     }"
                ).ToList();
            }
        }
    }

    public class MessageViewModel
    {
        public string From;
        public DateTime When;
        public string Message;
        public string ActionUrl;
    }
}
