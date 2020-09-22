using System.Reflection;
using Harmony;
using System.IO;

namespace CustomBackdrops
{
    public class CustomBackdrops
    {
        internal static string LogPath;
        internal static string ModDirectory;

        // BEN: DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        internal static int DebugLevel = 1;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;
            LogPath = Path.Combine(ModDirectory, "CustomBackdrops.log");

            Logger.Initialize(LogPath, DebugLevel, ModDirectory, nameof(CustomBackdrops));

            HarmonyInstance harmony = HarmonyInstance.Create("de.mad.CustomBackdrops");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}

