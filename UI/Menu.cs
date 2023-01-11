using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace MonoCraft.UI
{
    public class Menu
    {
        public Vector2 Position { get; init; }
        public Vector2 Center { get; init; }
        public Vector2 Size { get; init; }
        public Rectangle Bounds { get; init; }

        private IEnumerable<IUIElement> Items { get; init; }
        private IEnumerable<Button> Buttons { get; init; }

        public Menu(Vector2 center, IEnumerable<IUIElement> items)
        {
            Items = items;
            Buttons = items.OfType<Button>();
            Center = center;

            var greatestItemWidth = Items.Max(item => item.Size.X);
            var totalHeight = Items.Sum(item => item.Size.Y);

            float yOffset = -totalHeight / 2f;
            float gap = 4.0f;

            foreach (var item in Items)
            {
                item.Position = new(Center.X - item.Size.X / 2f, Center.Y + yOffset);
                yOffset += item.Size.Y + gap;
            }

            Size = new(greatestItemWidth, yOffset);
            Position = Center - Size / 2f;
            Bounds = Items.Select(item => item.Bounds).Aggregate(Rectangle.Union);
        }

        public void HandleInput(MouseState mouseState, MouseState previousMouseState)
        {
            foreach (var button in Buttons)
                button.HandleInput(mouseState, previousMouseState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in Items)
            {
                item.Draw(spriteBatch);
            }
        }
    }
}
