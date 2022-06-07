using AutoMapper;
using System.Reflection;

namespace Application.Common.Mappings
{
    /// <summary>
    /// Class MappingProfile
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes new instance of <see cref="MappingProfile" /> class.
        /// </summary>
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Adds mappings within assembly
        /// </summary>
        /// <param name="assembly">
        /// <see cref="Assembly"/>
        /// </param>
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))).ToList();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
