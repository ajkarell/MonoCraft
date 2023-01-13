using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoCraft.UI
{
    public class Button : IUIElement
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Center => Position + Size / 2f;
        public Vector2 Size => new(128, 32);
        public Rectangle Bounds => new(Position.ToPoint(), Size.ToPoint());

        public event Action OnClick;

        private Texture2D BackgroundTexture { get; set; }

        private static Color NormalTint = Color.White;
        private static Color HoverTint = new(0.9f, 0.9f, 0.9f);

        private bool isHoveredOver = false;

        public Button(string text, SpriteFont font, Texture2D backgroundTexture)
        {
            Text = text;
            Font = font;
            BackgroundTexture = backgroundTexture;
        }

        public void HandleInput(MouseState mouseState, MouseState previousMouseState)
        {
            isHoveredOver = Bounds.Contains(mouseState.Position);

            if (!isHoveredOver)
                return;

            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                OnClick?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackgroundTexture, Bounds, isHoveredOver ? HoverTint : NormalTint);
            spriteBatch.DrawString(Font, Text, Center - Font.MeasureString(Text) / 2f, Color.White);
        }
    }
}
