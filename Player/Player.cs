using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class Player : GameComponent, IDebugRowProvider
{
    public float MovementSpeed => 16.0f;
    public Vector3 Position { get; private set; }
    public Vector3Int ChunkCoordinate { get; private set; }
    public Vector3 Velocity { get; private set; }

    public Matrix ViewMatrix { get; private set; }

    public Player(Game game) : base(game)
    {
        Position = Vector3.Zero;
        Velocity = Vector3.Zero;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        Velocity = Vector3.Zero;

        HandleInput();
        if (Velocity != Vector3.Zero)
        {
            Position += Velocity * gameTime.GetDeltaTimeSeconds();
            ChunkCoordinate = Position.AsChunkCoordinate();
        }

        ViewMatrix = Matrix.CreateLookAt(Position, Position + Vector3.Forward, Vector3.Up);
    }

    void HandleInput() {
        int signX = 0, signY = 0, signZ = 0;

        if (Keyboard.GetState().IsKeyDown(Keys.W))
            signZ += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.S))
            signZ -= 1;

        if (Keyboard.GetState().IsKeyDown(Keys.D))
            signX += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.A))
            signX -= 1;
            
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
            signY += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            signY -= 1;

        if (signX != 0 || signY != 0 || signZ != 0)
        {
            var movementDirection = Vector3.Normalize(Vector3.Right * signX + Vector3.Up * signY + Vector3.Forward * signZ);
            Velocity = movementDirection * MovementSpeed;
        }
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"Pos: {Position}";
        yield return $"ChunkC: {ChunkCoordinate}";
    }
}
