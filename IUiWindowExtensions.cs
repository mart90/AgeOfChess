using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace AgeOfChess
{
    static class IUiWindowExtensions
    {
        public static IUiPart GetUiPartByLocation<T>(this T uiComponent, Point location) where T : IUiWindow
        {
            var matchedParts = uiComponent.UiParts.Where(e => e.LocationIncludesPoint(location));

            if (!matchedParts.Any())
            {
                return null;
            }
            if (matchedParts.Count() == 1)
            {
                return matchedParts.First();
            }

            // Matched multiple UI parts. Find which center we are nearest
            return matchedParts
                .Where(e => Math.Abs(location.Y - e.Center.Y) == matchedParts.Min(e => Math.Abs(location.Y - e.Center.Y)))
                .Single();
        }
    }
}
