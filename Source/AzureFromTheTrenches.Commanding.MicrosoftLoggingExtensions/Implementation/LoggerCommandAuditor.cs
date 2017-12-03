using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions.Implementation
{
    class LoggerCommandAuditor : ICommandAuditor
    {
        private readonly LogLevel _normalLogLevel;
        private readonly LogLevel _executionFailureLogLevel;
        private readonly ILogger<LoggerCommandAuditor> _logger;

        public LoggerCommandAuditor(
            ILoggerFactory loggerFactory,
            ILogLevelProvider logLevelProvider)
        {
            _normalLogLevel = logLevelProvider.NormalLogLevel;
            _executionFailureLogLevel = logLevelProvider.ExecutionFailureLogLevel;
            _logger = loggerFactory.CreateLogger<LoggerCommandAuditor>();
        }

        public Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
        {
            LogLevel level = _normalLogLevel;
            if (auditItem.Type == AuditItem.ExecutionType && !(auditItem.ExecutedSuccessfully ?? true))
            {
                level = _executionFailureLogLevel;
            }
            _logger.Log(level, (EventId)0, (object)new FormattedLogValues("command:{0}\nid:{1}\nstage:{2}\ncorrelation ID {3}", auditItem.CommandType, auditItem.CommandId, auditItem.Type, auditItem.CorrelationId), null, (state, ex) => state.ToString());
            return Task.FromResult(0);
        }
    }
}
