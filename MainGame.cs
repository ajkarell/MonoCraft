using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoCraft;

public class MainGame : Game
{
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;

    private readonly Player player;
    private World world;

    private Effect effect;

    public static Matrix ProjectionMatrix;
    public static Vector2 ScreenCenter;

    private TextureArray textureArray;

    private readonly List<IDebugRowProvider> debugRowProviders = new();

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.SynchronizeWithVerticalRetrace = false;
        graphics.GraphicsProfile = GraphicsProfile.HiDef; //for shaders to work

        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;

        graphics.SynchronizeWithVerticalRetrace = false;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / 144.0));

        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        player = new Player(this);

        Components.Add(player);
        
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (_, _) =>
        {
            ProjectionMatrix = CalculateProjectionMatrix();
            ScreenCenter = new(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
        };
    }

    protected override void Initialize()
    {
        foreach (IDebugRowProvider debugRowProvider in Components)
        {
            debugRowProviders.Add(debugRowProvider);
        }

        ScreenCenter = new(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

        ProjectionMatrix = CalculateProjectionMatrix();

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

        var chunkMeshes = world.GetChunkMeshesDueRender();
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

    Matrix CalculateProjectionMatrix()
        => Matrix.CreatePerspectiveFieldOfView(60.0f * (MathF.PI / 180.0f), (float)graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight, 0.01f, 100_000f);

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
