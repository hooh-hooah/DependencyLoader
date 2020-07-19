using System.Text.RegularExpressions;

namespace IL_DependencyLoader
{
    public static class Cursed
    {
        // fucking shit list
        private static readonly Regex CursedRegex = new Regex(@"abdata|add[0-9]+|studio[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsCursedManifest(string manifest)
        {
            return manifest.IsNullOrEmpty() || manifest.IsNullOrWhiteSpace() || CursedRegex.IsMatch(manifest);
        }
    }
}