namespace Client
{
    public interface IFileCopier
	{
        void CopyFile(CommandOptions options, string realizedFileName);
    }
}

