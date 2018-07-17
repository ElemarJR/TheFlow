using System;

namespace TheFlow.Notifications.Application.Infrastructure.Mediatr.Exceptions
{
    public class MediatrPipelineException : Exception
    {
        public MediatrPipelineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
