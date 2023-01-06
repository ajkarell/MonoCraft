using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoCraft;

public class Player : GameComponent, IDebugRowProvider
{
    public Vector3 Position { get; private set; }
    public Vector3Int ChunkCoordinate { get; private set; }
    private Vector3Int previousWorldGenChunkCoordinate;

    public Vector3 Velocity { get; private set; }

    public Vector3 Forward => ViewMatrixInverted.Forward;
    public Vector3 Right => ViewMatrixInverted.Right;

    private Matrix _viewMatrix;

    public Matrix ViewMatrix
    {
        get { return _viewMatrix; }
        private set 
        { 
            _viewMatrix = value; 
            ViewMatrixInverted = Matrix.Invert(_viewMatrix); 
        }
    }

    public Matrix ViewMatrixInverted { get; private set; }

    public BoundingFrustum ViewFrustum { get; private set; }

    public delegate void OnWorldGenThresholdCrossedHandler();
    public event OnWorldGenThresholdCrossedHandler OnWorldGenThresholdCrossed;

    private Vector3 eulerAngles;
    private MouseState previousMouseState;

    private readonly float MovementSpeed = 16.0f;
    private float FastMovementSpeed => MovementSpeed * 3.0f;

    private readonly float LookSensitivity = 1.0f;

    public Player(MainGame game) : base(game)
    {
        Position = Vector3.Zero;
        ChunkCoordinate = Position.AsChunkCoordinate();
        previousWorldGenChunkCoordinate = ChunkCoordinate;
        Velocity = Vector3.Zero;
        eulerAngles = Vector3.Zero;

        ViewMatrix = CalculateViewMatrix();
        ViewFrustum = CalculateViewFrustum();
    }

    public override void Update(GameTime gameTime)
    {
        Velocity = Vector3.Zero;

        HandleInput();
        if (Velocity != Vector3.Zero)
        {
            Position += Velocity * gameTime.GetDeltaTimeSeconds();
            ChunkCoordinate = Position.AsChunkCoordinate();

            if (Math.Abs(ChunkCoordinate.X - previousWorldGenChunkCoordinate.X) >= Settings.WorldGenThresholdHorizontal
                || Math.Abs(ChunkCoordinate.Y - previousWorldGenChunkCoordinate.Y) >= Settings.WorldGenThresholdVertical
                || Math.Abs(ChunkCoordinate.Z - previousWorldGenChunkCoordinate.Z) >= Settings.WorldGenThresholdHorizontal)
            {
                OnWorldGenThresholdCrossed();
                previousWorldGenChunkCoordinate = ChunkCoordinate;
            }
        }

        ViewMatrix = CalculateViewMatrix();
        ViewFrustum = CalculateViewFrustum();
    }

    void HandleInput()
    {
        if (!Game.IsActive)
            return;

        var keyState = Keyboard.GetState();

        int signX = 0, signY = 0, signZ = 0;

        if (keyState.IsKeyDown(Keys.W))
            signZ += 1;
        if (keyState.IsKeyDown(Keys.S))
            signZ -= 1;

        if (keyState.IsKeyDown(Keys.D))
            signX += 1;
        if (keyState.IsKeyDown(Keys.A))
            signX -= 1;

        if (keyState.IsKeyDown(Keys.Space))
            signY += 1;
        if (keyState.IsKeyDown(Keys.LeftControl))
            signY -= 1;

        if (signX != 0 || signY != 0 || signZ != 0)
        {
            var movementDirection = Vector3.Normalize(Right * signX + Vector3.Up * signY + Forward * signZ);
            var movementSpeed = keyState.IsKeyDown(Keys.LeftShift) ? FastMovementSpeed : MovementSpeed;
            Velocity = movementDirection * movementSpeed;
        }

        var mouseState = Mouse.GetState();
        var mouseDelta = mouseState.Position - previousMouseState.Position;
        if (mouseDelta.X != 0 || mouseDelta.Y != 0)
        {
            eulerAngles.X -= mouseDelta.Y * LookSensitivity;
            eulerAngles.Y -= mouseDelta.X * LookSensitivity;
        }

        eulerAngles.X = Math.Clamp(eulerAngles.X, -89.9f, 89.9f);
        eulerAngles.Y = Math.Repeat(eulerAngles.Y, 0f, 360f);

        Mouse.SetPosition((int)MainGame.ScreenCenter.X, (int)MainGame.ScreenCenter.Y);

        previousMouseState = Mouse.GetState();
    }

    BoundingFrustum CalculateViewFrustum() => new(ViewMatrix * MainGame.ProjectionMatrix);

    Matrix CalculateViewMatrix()
    {
        var pitchRadians = MathHelper.ToRadians(eulerAngles.X);
        var yawRadians = MathHelper.ToRadians(eulerAngles.Y);

        var cosPitch = MathF.Cos(pitchRadians);
        var sinPitch = MathF.Sin(pitchRadians);
        var cosYaw = MathF.Cos(yawRadians);
        var sinYaw = MathF.Sin(yawRadians);

        var xAxis = new Vector3(cosYaw, 0, -sinYaw);
        var yAxis = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
        var zAxis = new Vector3(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw);

        var dotX = Vector3.Dot(xAxis, Position);
        var dotY = Vector3.Dot(yAxis, Position);
        var dotZ = Vector3.Dot(zAxis, Position);

        return new Matrix(
            new(xAxis.X, yAxis.X, zAxis.X, 0),
            new(xAxis.Y, yAxis.Y, zAxis.Y, 0),
            new(xAxis.Z, yAxis.Z, zAxis.Z, 0),
            new(-dotX, -dotY, -dotZ, 1));
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"Position: {Position}";
        yield return $"Chunk coordinate: {ChunkCoordinate}";
        yield return $"Angles: {eulerAngles}";
    }
}
