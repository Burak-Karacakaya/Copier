using System.Net.Http;
using System.Threading;
using System;
namespace Client
{
    public interface IOutputLogger
	{
		void LogInfo(string message);
		void LogError(string message);
		void LogWarning(string message);
		void LogDebug(string message);

	}

	
}

