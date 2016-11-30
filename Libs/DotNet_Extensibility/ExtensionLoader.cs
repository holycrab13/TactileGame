using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace tud.mci.extensibility
{
    public static class ExtensionLoader
    {

        /// <summary>
        /// Loads all extension assemblies from the given base directory and all his sub folders.
        /// </summary>
        /// <param name="t">The Type to search for.</param>
        /// <param name="baseDirectory">The base directory.</param>
        /// <returns>Dictionary of key [EXT_(subfoldername)] and list of available class types</returns>
        public static Dictionary<String, List<Type>> LoadAllExtensions(Type t, string baseDirectory)
        {
            return loadAllExtensions(t, baseDirectory, "EXT");
        }

        /// <summary>
        /// Loads all extension assemblies from the given base directory and all his sub folders.
        /// </summary>
        /// <param name="t">The Type to search for.</param>
        /// <param name="baseDirectory">The base directory.</param>
        /// <param name="prefix">The prefix added to the directory name (normally the parents directory key).</param>
        /// <returns>
        /// Dictionary of key [EXT_(subfoldername)] and list of available class types
        /// </returns>
        static Dictionary<String, List<Type>> loadAllExtensions(Type t, string baseDirectory, string prefix)
        {
            Dictionary<String, List<Type>> dic = new Dictionary<String, List<Type>>();
            if (t != null && Directory.Exists(baseDirectory))
            {
                //list all directly added types (without sub folder structure)
                var baseypes = LoadTypes(t, baseDirectory);

                if (baseypes != null && baseypes.Count > 0)
                {
                    dic.Add(prefix, baseypes);
                }

                // list all sub directories
                var subDirs = Directory.GetDirectories(baseDirectory);
                if (subDirs != null && subDirs.Length > 0)
                {
                    foreach (var subDir in subDirs)
                    {
                        try
                        {
                            if (Directory.Exists(subDir))
                            {
                                string name = new DirectoryInfo(subDir).Name;
                                name = (String.IsNullOrWhiteSpace(prefix) ? "" : (prefix + "_")) + name;
                                var subs = loadAllExtensions(t, subDir, name);

                                //combine the subs to the org
                                if (subs != null && subs.Count > 0)
                                {
                                    dic = dic.Concat(subs).ToDictionary(x => x.Key, x => x.Value);
                                }
                            }
                        }
                        catch(Exception){}
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// Search the dll (assemblies) and return them.
        /// </summary>
        /// <param name="t">The Type to search for.</param>
        /// <param name="searchPath">The directory to search in.</param>
        /// <returns>list of available types</returns>
        public static List<Type> LoadTypes(Type t, string searchPath)
        {
            string[] dllFilenames = System.IO.Directory.GetFiles(searchPath, "*.dll");
            List<Type> types = new List<Type>();

            foreach (string filename in dllFilenames)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(filename);

                    Type[] typesInAssembly = asm.GetTypes();
                    foreach (Type type in typesInAssembly)
                    {
                        if (null != type.GetInterface(t.FullName))
                        {
                            types.Add(type);
                        }
                    }
                }
                catch (BadImageFormatException)
                {
                    // Not a valid assembly, move on
                }
                catch (Exception) { }
            }

            // Used to tell Main how many types we loaded
            return types;
        }


        /// <summary>
        /// Gets the current executing DLL's path.
        /// </summary>
        /// <returns>the full path and name of the currently executed dll (assembly)</returns>
        public static string GetCurrentDllPath()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return path;
        }

        /// <summary>
        /// Gets the current executing DLL's directory.
        /// </summary>
        /// <returns>the storage directory of the currently executed dll (assembly)</returns>
        public static string GetCurrentDllDirectory()
        {
            string path = GetCurrentDllPath();
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Creates an Object of the requested type.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>a typed objects or null</returns>
        public static Object CreateObjectFromType(Type t)
        {
            if (t != null)
            {
                try
                {
                    return Activator.CreateInstance(t);
                }
                catch (Exception)
                { }
            }
            return null;
        }
    }
}
