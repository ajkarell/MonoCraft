﻿using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoMaa;

public class MainGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;

    private World world;
    private Player player;

    private Effect effect;

    public static Matrix ProjectionMatrix;
    private static Vector2 screenCenter;
    private TextureArray textureArray;

    private List<IDebugRowProvider> debugRowProviders = new();

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.SynchronizeWithVerticalRetrace = false;
        graphics.GraphicsProfile = GraphicsProfile.HiDef; //for shaders to work

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 144.0f);

        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        player = new Player(this);

        Components.Add(player);

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (_, _) =>
        {
            ProjectionMatrix = GetProjectionMatrix();
            screenCenter = GetScreenCenter();
        };
    }

    protected override void Initialize()
    {
        foreach (IDebugRowProvider debugRowProvider in Components)
        {
            debugRowProviders.Add(debugRowProvider);
        }

        ProjectionMatrix = GetProjectionMatrix();
        screenCenter = GetScreenCenter();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("DebugFont");

        effect = Content.Load<Effect>("Main");
        effect.Parameters["Projection"].SetValue(ProjectionMatrix);

        var textureCount = Block.RegisterBlockTextures();
        textureArray = new TextureArray(GraphicsDevice, 16, 16, textureCount);
        textureArray.LoadTexturesFromContent(Content);
        effect.Parameters["TextureArray"].SetValue(textureArray);

        world = new World(player);
        debugRowProviders.Add(world);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DeepSkyBlue);

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        effect.Parameters["View"].SetValue(player.ViewMatrix);

        var chunkMeshes = world.GetChunkMeshes();
        foreach (var mesh in chunkMeshes)
        {
            effect.Parameters["World"].SetValue(mesh.WorldMatrix);
            //effect.Parameters["WorldInverseTranspose"].SetValue(mesh.InverseTransposeWorldMatrix);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    mesh.Vertices,
                    0,
                    mesh.VertexCount,
                    mesh.Indices,
                    0,
                    mesh.TriangleCount
                );
            }
        }

        base.Draw(gameTime);

        spriteBatch.Begin();
        DrawDebugUi(gameTime);
        spriteBatch.End();
    }

    public void CenterMouse()
    {
        Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);
    }

    Matrix GetProjectionMatrix()
        => Matrix.CreatePerspectiveFieldOfView(60.0f * (MathF.PI / 180.0f), (float)graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight, 0.01f, 100_000f);
    Vector2 GetScreenCenter()
        => new(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

    private void DrawDebugUi(GameTime gameTime)
    {
        var debugText = new StringBuilder();

        var fps = 1.0f / gameTime.GetDeltaTimeSeconds();
        debugText.AppendLine($"FPS: {fps:0}");

        foreach (var debugRowProvider in debugRowProviders)
        {
            foreach (var debugRow in debugRowProvider.GetDebugRows())
            {
                debugText.AppendLine(debugRow);
            }
        }

        spriteBatch.DrawString(font, debugText, new Vector2(10, 10), Color.White);
    }
}
