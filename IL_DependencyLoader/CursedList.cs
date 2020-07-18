using System.Text.RegularExpressions;

namespace IL_DependencyLoader
{
    public static class CursedList
    {
        // fucking shit list
        public static readonly Regex cursedRegex = new Regex(@"abdata|add[0-9]+|studio[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsCursedManifest(string manifest)
        {
            return manifest.IsNullOrEmpty() || manifest.IsNullOrWhiteSpace() || cursedRegex.IsMatch(manifest);
        }
    }
}