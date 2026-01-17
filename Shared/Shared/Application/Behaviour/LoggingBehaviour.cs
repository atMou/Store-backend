using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Shared.Application.Behaviour;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            typeof(TRequest), typeof(TResponse), request);

        var timer = Stopwatch.StartNew();
        var response = next(cancellationToken);
        timer.Stop();

        if (timer.Elapsed.Seconds > 2)
        {
            logger.LogInformation(
                "[END] Handle request={Request} - Response={Response} - ElapsedMilliseconds={ElapsedMilliseconds}ms",
                typeof(TRequest), typeof(TResponse), timer.ElapsedMilliseconds);
        }

        return response;
    }
}
