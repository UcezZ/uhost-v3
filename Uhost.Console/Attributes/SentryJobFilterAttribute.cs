using Hangfire.Common;
using Hangfire.States;
using Sentry;
using Uhost.Console.Common;

namespace Uhost.Console.Attributes
{
    public class SentryJobFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        public void OnStateElection(ElectStateContext context)
        {
            if (context?.CandidateState is FailedState failedState)
            {
                SentrySdk.CaptureException(new HangfireTaskFailedException(failedState, context));
            }
        }
    }
}
