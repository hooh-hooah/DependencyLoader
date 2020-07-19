#if REFLECTION_LIB_WIP
namespace IL_DependencyLoader
{
    public static class RefUtil
    {
        public static object GetStaticField<T>(this T obj, string fieldName)
        {
            var bindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = typeof(T).GetField(fieldName, bindFlags);
            return field.GetValue(null);
        }

        public static void SetStaticField<T>(this T obj, string fieldName, object value)
        {
            var bindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = typeof(T).GetField(fieldName, bindFlags);
            field.SetValue(null, value);
        }

        public static object GetInstanceField<T>(this T obj, string fieldName)
        {
            return typeof(T)
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(obj);
        }

        public static void SetInstanceField<T>(this T obj, string fieldName, object value)
        {
            var prop = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null) prop.SetValue(obj, value);
            else throw new MissingFieldException($"Field \"{fieldName}\" does not exists.");
        }

        public static void PrivateSetter<T>(this T obj, string fieldName, params object[] args)
        {
            typeof(T).GetProperty(fieldName)?.SetMethod.Invoke(obj, args);
        }

        public static object InvokeMethod<T>(this T obj, string methodName, params object[] args)
        {
            var method = typeof(T).GetTypeInfo().GetDeclaredMethod(methodName);
            return method.Invoke(obj, args);
        }
    }
}
#endif