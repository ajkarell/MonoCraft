using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCraft.UI
{
    public class Label : IUIElement
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size => Font.MeasureString(Text);
        public Rectangle Bounds => new(Position.ToPoint(), Size.ToPoint());

        public Label(string text, SpriteFont font)
        {
            Text = text;
            Font = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position, Color.White);
        }
    }
}
