using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Helpers
{
    // This is the helper class to the BlogDescriptionPlaceholder enum
    public static class BlogDescriptionPlaceholderText
    {
        // Extension method, lets you call myEnum.ToText()
        public static string ToText(this BlogDescriptionPlaceholder p) => p switch
        {
            BlogDescriptionPlaceholder.OmniscientSphere => "According to my analysis, this object has a high intellectual capacity. " +
            "But if it's so smart, why won't it pass on some of its wisdom to me?",
            BlogDescriptionPlaceholder.StellarOrb => "The technology behind this impressive gadget is totally unknown to my people... " +
            "It appears to replicate the intense solar beams of the sun. Space exploration has given me a ghostly pallor.",
            BlogDescriptionPlaceholder.OpticalIllustration => "I've noticed that some of the objects we collect have undeniable similarities. " +
            "Perhaps these treasures were revered by an ancient civilization.",
            _ => "No data entry for this blog"
        };

        public static string RandomText()
        {
            var values = (BlogDescriptionPlaceholder[])Enum.GetValues(typeof(BlogDescriptionPlaceholder));
            var pick = values[Random.Shared.Next(values.Length)];
            return pick.ToText();
        }
    }
}
