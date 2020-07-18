using System.Reflection;

namespace IL_DependencyLoader
{
    public static class RefUtil
    {
        public static object GetInstanceField<T>(T instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = typeof(T).GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}