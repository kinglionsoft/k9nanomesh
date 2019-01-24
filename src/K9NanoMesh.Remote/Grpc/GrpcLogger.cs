using System;
using Microsoft.Extensions.Logging;
using IGrpcLogger = global::Grpc.Core.Logging.ILogger;

namespace K9NanoMesh.Remote
{
    public class GrpcLogger: IGrpcLogger
    {
        private readonly ILogger _logger;

        public GrpcLogger(ILogger logger)
        {
            _logger = logger;
        }

        public IGrpcLogger ForType<T>()
        {
            return this;
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            _logger.LogDebug(format, formatArgs);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            _logger.LogInformation(format, formatArgs);
        }

        public void Warning(string message)
        {
            _logger.LogWarning(message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            _logger.LogWarning(format, formatArgs);
        }

        public void Warning(Exception exception, string message)
        {
            _logger.LogWarning(exception, message);
        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            _logger.LogError(format, formatArgs);
        }

        public void Error(Exception exception, string message)
        {
            _logger.LogError(exception, message);
        }
    }
}