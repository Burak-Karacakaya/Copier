using System;
namespace CopierPluginBase
{
	public interface IPreCopyEventListener
	{
		void OnPreCopy(string filePath);
	}
}

