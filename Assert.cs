namespace UserManagementAPI
{
    public static class Assert
    {
        public static string NotNull(object obj)
        {
            if (obj == null)
            {
                return $"Assert.NotNull: failed";
            }
            else
            {
                return $"Assert.NotNull: Object \"{obj}\" succeeded";
            }
        }

        public static string Equal(object expected, object actual)
        {
            if (!expected.Equals(actual))
            {
                return $"Assert.Equal \"{expected}\"==\"{actual}\" failed\"";
            }
            else
            {
                return $"Assert.Equal \"{expected}\"==\"{actual}\" succeeded";
            }
        }
    }
}