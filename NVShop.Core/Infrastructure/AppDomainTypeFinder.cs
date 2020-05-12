namespace NVShop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public class AppDomainTypeFinder : ITypeFinder
    {
        #region Private Fields

        private readonly bool _ignoreReflectionErrors = true;
        
        #endregion

        #region Properties

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App => AppDomain.CurrentDomain;

        /// <summary>
        ///     Gets or sets wether NVShop should iterate assemblies in the app domain when loading NVShop types. Loading
        ///     patterns are applied when loading these assemblies.
        /// </summary>
        public bool LoadAppDomainAssemblies { get; set; } = true;

        /// <summary>Gets or sets assemblies loaded at startup in addition to those loaded in the AppDomain.</summary>
        public IList<string> AssemblyNames { get; set; } = new List<string>();

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern { get; set; } = @"^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^Autofac|^AutoMapper|^dotless|^EntityFramework|^EPPlus|^Fasterflect|^FiftyOne|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^Antlr3|^Recaptcha|^FluentValidation|^ImageResizer|^itextsharp|^MiniProfiler|^Newtonsoft|^Pandora|^WebGrease|^Noesis|^DotNetOpenAuth|^Facebook|^LinqToTwitter|^PerceptiveMCAPI|^CookComputing|^GCheckout|^Mono\.Math|^Org\.Mentalis|^App_Web|^BundleTransformer|^ClearScript|^JavaScriptEngineSwitcher|^Glimpse|^Ionic|^App_GlobalResources|^AjaxMin|^MefContrib|^WebActivatorEx|^Elmah|^Common.Logging|^ElasticSearch|^Kendo|^LinqKit|^Nest|^Owin|^Portable.Licensing|^Quartz|^RazorEngine|^RazorGenerator|^RequireJsNet|^TwitterBootstrapMVC|^UAParser|^T4MVCExtensions|^protobuf-net|^FrameworkSystems|.*NV.ERP|.*Root_I";

        /// <summary>
        ///     Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to
        ///     increase performance you might want to configure a pattern that includes assemblies and your own.
        /// </summary>
        /// <remarks>
        ///     If you change this so that NVShop assemblies arn't investigated (e.g. by not including something like
        ///     "^|..." you may break core functionality.
        /// </remarks>
        public string AssemblyRestrictToLoadingPattern { get; set; } = ".*";

        #endregion

        /// <summary>Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.</summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assembly in App.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>Adds specificly configured assemblies.</summary>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assemblyName in AssemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
        /// <param name="assemblyFullName">The name of the assembly to check.</param>
        /// <returns>True if the assembly should be loaded into NVShop.</returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern) && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
        /// <param name="assemblyFullName">The assembly name to match.</param>
        /// <param name="pattern">The regular expression pattern to match against the assembly name.</param>
        /// <returns>True if the pattern matches the assembly name.</returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>Makes sure matching assemblies in the supplied folder are loaded in the app domain.</summary>
        /// <param name="directoryPath">The physical path to a directory containing dlls to load in the app domain.</param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            var loadedAssemblyNames = new List<string>();
            foreach (var a in GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (var dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    if (Matches(an.FullName) && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        App.Load(an);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();

                var interfaceTypes = type.FindInterfaces((t, criteria) => ((Type) criteria).IsAssignableFrom(t) || t.IsGenericType && ((Type) criteria).IsAssignableFrom(t.GetGenericTypeDefinition()), genericTypeDefinition);
                var baseTypes = type.FindBaseTypes((t, criteria) => ((Type)criteria).IsAssignableFrom(t) || t.IsGenericType && ((Type)criteria).IsAssignableFrom(t.GetGenericTypeDefinition()), genericTypeDefinition);

                foreach (var implementedTypes in interfaceTypes.Union(baseTypes))
                {
                    if (!implementedTypes.IsGenericType)
                    {
                        continue;
                    }

                    if (genericTypeDefinition.IsAssignableFrom(implementedTypes.GetGenericTypeDefinition()))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #region Nested Types

        #region Internal Attributed Assembly class

        private class AttributedAssembly
        {
            #region Properties

            internal Assembly Assembly { get; set; }
            internal Type PluginAttributeType { get; set; }

            #endregion
        }

        #endregion

        #endregion

        #region ITypeFinder

        public Type FindType(string fullName)
        {
            return GetAssemblies()
                .Select(a => a.GetType(fullName))
                .FirstOrDefault(x => x != null);
        }


        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();

            try
            {
                foreach (var a in assemblies)
                {
                    Type[] types = null;
                    try
                    {
                        types = a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        if (!_ignoreReflectionErrors)
                        {
                            throw;
                        }
                        types = ex.Types.Where(t => t != null)
                            .ToArray();
                    }

                    foreach (var t in types)
                    {
                        if (assignTypeFrom.IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        {
                            if (t.IsInterface)
                            {
                                continue;
                            }

                            if (onlyConcreteClasses)
                            {
                                if (t.IsClass && !t.IsAbstract)
                                {
                                    result.Add(t);
                                }
                            }
                            else
                            {
                                result.Add(t);
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                {
                    msg += e.Message + Environment.NewLine;
                }

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            return result;
        }

        public IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute
        {
            var found = FindAssembliesWithAttribute<TAssemblyAttribute>();
            return FindClassesOfType<T>(found, onlyConcreteClasses);
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>()
        {
            return FindAssembliesWithAttribute<T>(GetAssemblies());
        }

        /// <summary>
        ///     Caches attributed assembly information so they don't have to be re-read
        /// </summary>
        private readonly List<AttributedAssembly> _attributedAssemblies = new List<AttributedAssembly>();

        /// <summary>
        ///     Caches the assembly attributes that have been searched for
        /// </summary>
        private readonly List<Type> _assemblyAttributesSearched = new List<Type>();

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies)
        {
            //check if we've already searched this assembly);)
            if (!_assemblyAttributesSearched.Contains(typeof(T)))
            {
                var foundAssemblies = (from assembly in assemblies let customAttributes = assembly.GetCustomAttributes(typeof(T), false) where customAttributes.Any() select assembly).ToList();
                //now update the cache
                _assemblyAttributesSearched.Add(typeof(T));
                foreach (var a in foundAssemblies)
                {
                    _attributedAssemblies.Add(new AttributedAssembly
                    {
                        Assembly = a,
                        PluginAttributeType = typeof(T)
                    });
                }
            }

            //We must do a ToList() here because it is required to be serializable when using other app domains.
            return _attributedAssemblies.Where(x => x.PluginAttributeType.Equals(typeof(T)))
                .Select(x => x.Assembly)
                .ToList();
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath)
        {
            var assemblies = (from f in Directory.GetFiles(assemblyPath.FullName, "*.dll") select Assembly.LoadFrom(f) into assembly let customAttributes = assembly.GetCustomAttributes(typeof(T), false) where customAttributes.Any() select assembly).ToList();
            return FindAssembliesWithAttribute<T>(assemblies);
        }

        /// <summary>Gets tne assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the SmartStore factory.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
            {
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            }
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }

        #endregion
    }
}