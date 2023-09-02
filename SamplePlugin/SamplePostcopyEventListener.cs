using System;
using CopierPluginBase;

namespace SamplePlugin
{
	public class SamplePostcopyEventListener : IPostCopyEventListener
	{
        //todo ignore abstract plugin classes by checking if they are not abstract during plugin loading
        public void OnPostCopy(string filePath)
        {
            Console.WriteLine("SamplePostCopyEventListener is execute");

        }
    }
}

