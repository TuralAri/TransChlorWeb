namespace TransChlorApi.Utils;

public class PathUtils
{
    public static string ResolvePath(string inputPath)
    {
        if (Path.IsPathRooted(inputPath))
            return inputPath;

        var baseDir = "/data/shared";

        return Path.Combine(baseDir, inputPath);
    }
}