using System.Collections.Generic;

namespace Naninovel.Spreadsheet
{
    public class LocaleTagsCache
    {
        protected virtual Dictionary<int, string> KeyToTag { get; }

        public LocaleTagsCache ()
        {
            KeyToTag = new Dictionary<int, string>();
        }

        public virtual string Get (int cacheKey, string content)
        {
            if (KeyToTag.TryGetValue(cacheKey, out var result))
                return result;

            var tag = content?.GetAfter("<")?.GetBefore(">");
            if (string.IsNullOrWhiteSpace(tag))
                throw new Error($"Failed to extract localization tag from `{content}`. Try re-generating localization documents.");

            KeyToTag[cacheKey] = tag;
            return tag;
        }
    }
}
