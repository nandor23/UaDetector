namespace UaDetector.ReadmeUpdater.Utilities;

public static class PathLocator
{
    public static string GetReadmePath()
    {
        var currentPath = Directory.GetCurrentDirectory();

        while (currentPath is not null)
        {
            var readmePath = Path.Combine(currentPath, "README.md");

            if (File.Exists(readmePath))
            {
                return readmePath;
            }

            currentPath = Directory.GetParent(currentPath)?.FullName;
        }

        throw new FileNotFoundException("Could not locate README.md");
    }

    public static string GetDocsPath()
    {
        var readmePath = GetReadmePath();
        var solutionRoot = Path.GetDirectoryName(readmePath)!;
        return Path.Combine(solutionRoot, "docs", "detection-capabilities");
    }
}
