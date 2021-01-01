using System;

namespace Cirrus.Module.CatchEmAll.Helper
{
    internal static class Ensure
    {
        public static void NotNull(object value, string name)
        {
            if (value == null)
            {
                throw new Exception($"'{name}' must not be null!");
            }
        }
    }
}
