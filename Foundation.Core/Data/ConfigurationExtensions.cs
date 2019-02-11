using System;
using System.IO;

namespace Foundation.Core.Data
{
    public static class ConfigurationExtensions
    {
        // TODO: deprecate this
        public static string GetFullPath(this GlobalSettings config, string secondPath)
        {
            if (secondPath == null)
                return null;

            if (secondPath.Contains("file://") || secondPath.Contains("http://") || secondPath.Contains("https://"))
                return new Uri(secondPath).LocalPath;

            return Path.Combine(config.RootFolder, secondPath);
        }

        public static string GetFullPath(this GlobalSettings config, Uri secondPath)
        {
            return secondPath?.LocalPath;

        }
    }
    
}
