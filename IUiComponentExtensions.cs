using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace AgeOfChess
{
    static class IUiComponentExtensions
    {
        public static Button GetButtonByLocation<T>(this T uiComponent, Point location) where T : IUiComponent
        {
            var matchedButtons = uiComponent.Buttons.Where(e => e.LocationIncludesPoint(location));

            if (!matchedButtons.Any())
            {
                return null;
            }
            if (matchedButtons.Count() == 1)
            {
                return matchedButtons.First();
            }

            // Matched multiple buttons. Find which center we are nearest
            return matchedButtons
                .Where(e => Math.Abs(location.Y - e.Center.Y) == matchedButtons.Min(e => Math.Abs(location.Y - e.Center.Y)))
                .Single();
        }
    }
}
