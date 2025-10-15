using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace JobHuntAPI.Utility
{
	public class HttpContextEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
	{


		public HttpContextEnricher() : this(new HttpContextAccessor())
		{
			
		}


		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var httpContext = httpContextAccessor.HttpContext;
			if (httpContext == null) return;
			var userName = httpContext.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";
			var logProp = new LogEventProperty("User", new Serilog.Events.ScalarValue(userName));
			logEvent.AddOrUpdateProperty(logProp);
		}
	}

	public static class HttpContextLoggerExtensions
	{
		public static LoggerConfiguration WithHttpContextEnricher(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration)
		{
			return loggerEnrichmentConfiguration.With(new HttpContextEnricher());
		}
	}
}
