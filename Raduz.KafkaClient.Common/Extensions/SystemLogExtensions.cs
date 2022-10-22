using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Raduz.KafkaClient.Common.Extensions
{
	/// <summary>
	/// Logging extensions
	/// </summary>
	public static class SystemLogExtensions
	{
		/// <summary>
		/// Change kafka specific log level to microsoft log level
		/// </summary>
		/// <param name="syslogLevel"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static LogLevel ToLogLevel(this SyslogLevel syslogLevel)
		{
			switch (syslogLevel)
			{
			case SyslogLevel.Emergency:
				return LogLevel.Critical;
			case SyslogLevel.Alert:
				return LogLevel.Critical;
			case SyslogLevel.Critical:
				return LogLevel.Critical;
			case SyslogLevel.Error:
				return LogLevel.Error;
			case SyslogLevel.Warning:
				return LogLevel.Warning;
			case SyslogLevel.Notice:
				return LogLevel.Information;
			case SyslogLevel.Info:
				return LogLevel.Information;
			case SyslogLevel.Debug:
				return LogLevel.Debug;
			default:
				throw new ArgumentOutOfRangeException(nameof(syslogLevel), syslogLevel, null);
			}
		}
	}
}
