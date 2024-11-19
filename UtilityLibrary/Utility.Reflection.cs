using System;
using System.Reflection;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains commonly used properties and methods.
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// Gets the name of the calling assembly.
        /// </summary>
        /// <returns>The name of the assembly.</returns>
        public static string GetAssemblyName()
        {
            return GetAssemblyName(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the name of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly whose name to get.</param>
        /// <returns>The name of the assembly.</returns>
        public static string GetAssemblyName(Assembly assembly)
        {
            AssemblyTitleAttribute attribute = GetAttribute<AssemblyTitleAttribute>(assembly, false);
            return attribute != null ? attribute.Title : string.Empty;
        }

        /// <summary>
        /// Gets the version of the calling assembly.
        /// </summary>
        /// <returns>The version of the assembly.</returns>
        public static Version GetAssemblyVersion()
        {
            return GetAssemblyVersion(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the version of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly whose name to get.</param>
        /// <returns>The version of the assembly.</returns>
        public static Version GetAssemblyVersion(Assembly assembly)
        {
            return assembly.GetName().Version;
        }

        /// <summary>
        /// Gets the copyright information of the calling assembly.
        /// </summary>
        /// <returns>The copyright information of the assembly.</returns>
        public static string GetAssemblyCopyright()
        {
            return GetAssemblyCopyright(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the copyright information of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly whose copyright information to get.</param>
        /// <returns>The copyright information of the assembly.</returns>
        public static string GetAssemblyCopyright(Assembly assembly)
        {
            AssemblyCopyrightAttribute attribute = GetAttribute<AssemblyCopyrightAttribute>(assembly, false);
            return attribute != null ? attribute.Copyright : string.Empty;
        }

        /// <summary>
        /// Gets the company of the calling assembly.
        /// </summary>
        /// <returns>The company of the assembly.</returns>
        public static string GetAssemblyCompany()
        {
            return GetAssemblyCompany(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the company of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly whose company to get.</param>
        /// <returns>The company of the assembly.</returns>
        public static string GetAssemblyCompany(Assembly assembly)
        {
            AssemblyCompanyAttribute attribute = GetAttribute<AssemblyCompanyAttribute>(assembly, false);
            return attribute != null ? attribute.Company : string.Empty;
        }

        /// <summary>
        /// Searches for and returns the first attribute of the specified type on the target member.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to search for.</typeparam>
        /// <param name="attributeProvider">The <see cref="System.Reflection.ICustomAttributeProvider"/>
        /// whose attributes are to be searched.</param>
        /// <param name="inherit"><c>true</c> to search the member's inhertiance chain for the attribute; <c>false</c> otherwise.</param>
        /// <returns>The first attribute of the specified type found on the target member; <c>null</c> if none were found.</returns>
        public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(attributeProvider, inherit, 0);
        }

        /// <summary>
        /// Searches for and returns the first attribute of the specified type on the target member.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to search for.</typeparam>
        /// <param name="attributeProvider">The <see cref="System.Reflection.ICustomAttributeProvider"/>
        /// whose attributes are to be searched.</param>
        /// <param name="inherit"><c>true</c> to search the member's inhertiance chain for the attribute; <c>false</c> otherwise.</param>
        /// <param name="index">The zero-based index of the attribute to return; 0 returns the first attribute found, 1 the second, etc.</param>
        /// <returns>The first attribute of the specified type found on the target member; <c>null</c> if none were found.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The given index was larger than the number of attributes found minus 1.</exception>
        public static T GetAttribute<T>(ICustomAttributeProvider attributeProvider, bool inherit, int index) where T : Attribute
        {
            return GetAttribute(attributeProvider, typeof(T), inherit, index) as T;
        }

        /// <summary>
        /// Searches for and returns the first attribute of the specified type on the target member.
        /// </summary>
        /// <param name="attributeProvider">The <see cref="System.Reflection.ICustomAttributeProvider"/>
        /// whose attributes are to be searched.</param>
        /// <param name="attributeType">The <see cref="System.Type"/> that represents the type of the attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to search the member's inhertiance chain for the attribute; <c>false</c> otherwise.</param>
        /// <returns>The first attribute of the specified type found on the target member; <c>null</c> if none were found.</returns>
        public static Attribute GetAttribute(ICustomAttributeProvider attributeProvider, Type attributeType, bool inherit)
        {
            return GetAttribute(attributeProvider, attributeType, inherit, 0);
        }

        /// <summary>
        /// Searches for and returns the first attribute of the specified type on the target member.
        /// </summary>
        /// <param name="attributeProvider">The <see cref="System.Reflection.ICustomAttributeProvider"/>
        /// whose attributes are to be searched.</param>
        /// <param name="attributeType">The <see cref="System.Type"/> that represents the type of the attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to search the member's inhertiance chain for the attribute; <c>false</c> otherwise.</param>
        /// <param name="index">The zero-based index of the attribute to return; 0 returns the first attribute found, 1 the second, etc.</param>
        /// <returns>The first attribute of the specified type found on the target member; <c>null</c> if none were found.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The given index was larger than the number of attributes found less 1.</exception>
        public static Attribute GetAttribute(ICustomAttributeProvider attributeProvider, Type attributeType, bool inherit, int index)
        {
            bool hasAttribute = attributeProvider.IsDefined(attributeType, inherit);
            if (!hasAttribute)
            {
                return null;
            }

            object[] attributes = attributeProvider.GetCustomAttributes(attributeType, inherit);
            if (attributes != null && attributes.Length != 0)
            {
                return attributes[index] as Attribute;
            }
            else
            {
                return null;
            }
        }
    }
}