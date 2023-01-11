using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCraft.UI
{
    public interface IUIElement
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; }
        public Rectangle Bounds { get; }
        public void Draw(SpriteBatch spriteBatch);
    }
}
