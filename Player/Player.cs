using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMaa;

public class Player : GameComponent, IDebugRowProvider
{
    public Vector3 Position { get; private set; }
    public Vector3Int ChunkCoordinate { get; private set; }
    public Vector3 Velocity { get; private set; }

    public Vector3 Forward => ViewMatrixInverted.Forward;
    public Vector3 Right => ViewMatrixInverted.Right;

    public Matrix ViewMatrix { get; set; }
    private Matrix ViewMatrixInverted { get; set; }

    private Vector3 eulerAngles;
    MouseState previousMouseState;

    float MovementSpeed => 16.0f;
    float LookSensitivity => 3.0f;

    readonly MainGame game;

    public Player(MainGame game) : base(game)
    {
        this.game = game;
        Position = Vector3.Zero;
        Velocity = Vector3.Zero;
        eulerAngles = Vector3.Zero;
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

        ViewMatrix = GetViewMatrix();
        ViewMatrixInverted = Matrix.Invert(ViewMatrix);
    }

    void HandleInput() {
        if(!game.IsActive)
            return;

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
            var movementDirection = Vector3.Normalize(Right * signX + Vector3.Up * signY + Forward * signZ);
            Velocity = movementDirection * MovementSpeed;
        }

        var mouseState = Mouse.GetState();
        var mouseDelta = mouseState.Position - previousMouseState.Position;
        if (mouseDelta.X != 0 || mouseDelta.Y != 0)
        {
            eulerAngles.X -= mouseDelta.Y * LookSensitivity;
            eulerAngles.Y -= mouseDelta.X * LookSensitivity;
        }

        eulerAngles.X = MonoMath.Clamp(eulerAngles.X, -89.9f, 89.9f);
        eulerAngles.Y = MonoMath.Repeat(eulerAngles.Y, 0f, 360f);
            
        game.CenterMouse();

        previousMouseState = Mouse.GetState();
    }

    Matrix GetViewMatrix()
    {
        var pitchRadians = MathHelper.ToRadians(eulerAngles.X);
        var yawRadians = MathHelper.ToRadians(eulerAngles.Y);

        float cosPitch = MathF.Cos(pitchRadians);
        float sinPitch = MathF.Sin(pitchRadians);
        float cosYaw = MathF.Cos(yawRadians);
        float sinYaw = MathF.Sin(yawRadians);

        Vector3 xAxis = new Vector3(cosYaw, 0, -sinYaw);
        Vector3 yAxis = new Vector3(sinYaw*sinPitch, cosPitch, cosYaw*sinPitch);
        Vector3 zAxis = new Vector3(sinYaw*cosPitch, -sinPitch, cosPitch*cosYaw);
        float dotX = Vector3.Dot(xAxis, Position);
        float dotY = Vector3.Dot(yAxis, Position);
        float dotZ = Vector3.Dot(zAxis, Position);

        return new Matrix(
            new Vector4(xAxis.X, yAxis.X, zAxis.X, 0),
            new Vector4(xAxis.Y, yAxis.Y, zAxis.Y, 0),
            new Vector4(xAxis.Z, yAxis.Z, zAxis.Z, 0),
            new Vector4(-dotX, -dotY, -dotZ, 1));
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"Pos: {Position}";
        yield return $"ChunkC: {ChunkCoordinate}";
        yield return $"Angles: {eulerAngles}";
    }
}
