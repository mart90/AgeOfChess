using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace AgeOfChess
{
    static class IUiWindowExtensions
    {
        public static IUiPart GetUiPartByLocation<T>(this T uiWindow, Point location) where T : IUiWindow
        {
            var matchedParts = uiWindow.UiParts.Where(e => e.LocationIncludesPoint(location) && e.IsEnabled);

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

        public static string TextBoxValueByType<T>(this T uiWindow, TextBoxType type) where T : IUiWindow
        {
            return ((TextBox)uiWindow.UiParts.Single(e => e is TextBox tb && tb.Type == type)).Text;
        }

        public static void ReceiveKeyboardInput<T>(this T uiWindow, TextInputEventArgs args) where T : IUiWindow
        {
            var focusedTextBox = (TextBox)uiWindow.UiParts.SingleOrDefault(e => e is TextBox tb && tb.HasFocus);

            if (focusedTextBox == null)
            {
                return;
            }

            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Back && focusedTextBox.Text != "")
            {
                focusedTextBox.Text = focusedTextBox.Text[0..^1];
            }
            else if (char.IsLetterOrDigit(args.Character))
            {
                focusedTextBox.Text += args.Character;
            }
        }
    }
}
