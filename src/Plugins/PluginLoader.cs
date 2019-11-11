using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Plugins
{
    public static class PluginLoader
    {

        private static Assembly Load(string pluginLocation, bool loadUsingPluginLoadContext)
        {
            return loadUsingPluginLoadContext ?
                new PluginLoadContext(pluginLocation).LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)))
                : Assembly.LoadFile(pluginLocation);
        }

        public static IEnumerable<IPluginAssembly> CreateInstance(string pluginLocation, bool loadUsingPluginLoadContext=false)
        {
            int count = 0;
            var assembly = Load(pluginLocation, loadUsingPluginLoadContext);
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPluginAssembly).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is IPluginAssembly result)
                    {
                        count++;
                        yield return result;
                    }
                }
            }
            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements {typeof(IPluginAssembly).Name} in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}