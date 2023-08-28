namespace Client
{
    public class FileCopier : IFileCopier
    {
        public void CopyFile(string sourceDirectoryPath, string fileName, string targetDirectoryPath, bool overWriteTargetFile)
        {
            var absoluteSourceFilePath = Path.Combine(sourceDirectoryPath, fileName);
            var absoluteTargetFilePath = Path.Combine(targetDirectoryPath, fileName);

            if (File.Exists(absoluteTargetFilePath) && !overWriteTargetFile) return;

            File.Copy(absoluteSourceFilePath, absoluteTargetFilePath, overWriteTargetFile);
        }
    }
}

