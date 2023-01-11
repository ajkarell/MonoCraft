using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoCraft;

public class Player : GameComponent, IDebugRowProvider
{
    private Vector3 position;

    public Vector3 Position
    {
        get { return position; }
        private set
        {
            position = value;
            ChunkCoordinate = value.AsChunkCoordinate();
            ViewMatrix = Math.CalculateViewMatrix(value, EulerAngles);
        }
    }

    public Vector3Int ChunkCoordinate { get; private set; }

    private Vector3 eulerAngles;

    public Vector3 EulerAngles
    {
        get { return eulerAngles; }
        private set
        {
            eulerAngles = value;
            eulerAngles.X = Math.Clamp(value.X, -89.9f, 89.9f);
            eulerAngles.Y = Math.Repeat(value.Y, 0f, 360f);

            ViewMatrix = Math.CalculateViewMatrix(Position, value);
        }
    }

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
            ViewMatrixInverted = Matrix.Invert(value);
            ViewFrustum = new(value * MainGame.ProjectionMatrix);

            OnViewChanged?.Invoke();
        }
    }

    public Matrix ViewMatrixInverted { get; private set; }

    public BoundingFrustum ViewFrustum { get; private set; }

    public event Action OnWorldScanThresholdCrossed;

    public event Action OnViewChanged;

    private Vector3Int previousWorldGenChunkCoordinate;

    private readonly float MovementSpeed = 16.0f;
    private float FastMovementSpeed => MovementSpeed * 3.0f;

    private MouseState previousMouseState;

    public Player(Game game) : base(game)
    {
        Position = Vector3.Zero;

        Velocity = Vector3.Zero;
        EulerAngles = Vector3.Zero;

        previousWorldGenChunkCoordinate = ChunkCoordinate;
        previousMouseState = Mouse.GetState();
    }

    public override void Update(GameTime gameTime)
    {
        Velocity = Vector3.Zero;

        HandleInput(gameTime);

        if (Velocity != Vector3.Zero)
        {
            Position += Velocity * gameTime.GetDeltaTimeSeconds();

            if (Math.Abs(ChunkCoordinate.X - previousWorldGenChunkCoordinate.X) >= Settings.WorldGenThresholdHorizontal
                || Math.Abs(ChunkCoordinate.Y - previousWorldGenChunkCoordinate.Y) >= Settings.WorldGenThresholdVertical
                || Math.Abs(ChunkCoordinate.Z - previousWorldGenChunkCoordinate.Z) >= Settings.WorldGenThresholdHorizontal)
            {
                OnWorldScanThresholdCrossed();
                previousWorldGenChunkCoordinate = ChunkCoordinate;
            }
        }
    }

    void HandleInput(GameTime gameTime)
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
            EulerAngles -= new Vector3(mouseDelta.Y, mouseDelta.X, 0f) * Settings.LookSensitivity * gameTime.GetDeltaTimeSeconds();
        }

        MainGame.CenterMouse();

        previousMouseState = Mouse.GetState();
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"Position: {Position.FloorToInt()}";
        yield return $"Chunk coordinate: {ChunkCoordinate}";
        yield return $"Orientation: {EulerAngles.FloorToInt()}";
    }
}
