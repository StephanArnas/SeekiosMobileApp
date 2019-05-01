namespace SeekiosApp.Extension
{
    public static class JsonExtension
    {
        public static bool IsJson(this string jsonData)
        {
            return jsonData.Trim().Substring(0, 1).IndexOfAny(new[] { '[', '{' }) == 0;
        }
    }
}
