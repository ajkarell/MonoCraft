using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCraft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoCraft;

public class MainGame : Game
{
    public static Matrix ProjectionMatrix { get; private set; }

    private readonly GraphicsDeviceManager graphics;
    private static SpriteBatch spriteBatch;
    private static SpriteFont font;

    private static Vector2 screenCenter;

    private readonly Player player;
    private World world;

    private Effect effect;

    private TextureArray textureArray;

    private KeyboardState previousKeyState;
    private MouseState previousMouseState;

    private Texture2D buttonBackgroundTexture;
    private Menu menu;

    private const float NearPlane = 0.001f;
    private const float FarPlane = 100_000f;

    private float AspectRatio => (float)graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight;

    private double fps = 0;
    private readonly List<IDebugRowProvider> debugRowProviders = new();

    enum GameState
    {
        Playing,
        Paused,
        InSettingsMenu,
    }

    private GameState _currentGameState;
    private GameState CurrentGameState
    {
        get
        {
            return _currentGameState;
        }
        set
        {
            _currentGameState = value;
            menu = CreateMenuForCurrentGameState();
            IsMouseVisible = value != GameState.Playing;

            if (value == GameState.Playing)
                CenterMouse();
        }
    }

    public MainGame()
    {
        graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef, //for shaders to work

            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,

            SynchronizeWithVerticalRetrace = true
        };

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / 144.0));

        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        player = new Player(this, spawnPoint: new(0, 32, 0));

        Components.Add(player);

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (_, _) =>
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Settings.FieldOfView, AspectRatio, NearPlane, FarPlane);
            screenCenter = new(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            menu = CreateMenuForCurrentGameState();
        };

        CurrentGameState = GameState.Playing;
    }

    protected override void Initialize()
    {
        debugRowProviders.AddRange(Components.OfType<IDebugRowProvider>());

        screenCenter = new(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Settings.FieldOfView, AspectRatio, NearPlane, FarPlane);

        previousKeyState = Keyboard.GetState();
        previousMouseState = Mouse.GetState();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("DebugFont");

        var textureCount = Block.RegisterBlockTextures();
        textureArray = new TextureArray(GraphicsDevice, textureWidth: 16, textureHeight: 16, textureCount);
        textureArray.LoadTexturesFromContent(Content);

        effect = Content.Load<Effect>("Main");
        effect.Parameters["TextureArray"].SetValue(textureArray);
        effect.Parameters["Projection"].SetValue(ProjectionMatrix);

        player.OnViewChanged += () =>
        {
            effect.Parameters["View"].SetValue(player.ViewMatrix);
        };

        world = new World(player);
        debugRowProviders.Add(world);

        buttonBackgroundTexture = Content.Load<Texture2D>(@"UI\button");

        menu = CreateMenuForCurrentGameState();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
        {
            if (CurrentGameState != GameState.Playing)
                CurrentGameState = GameState.Playing;
            else
                CurrentGameState = GameState.Paused;
        }

        if (CurrentGameState == GameState.Playing)
            base.Update(gameTime);
        else
            menu.HandleInput(mouseState, previousMouseState);

        previousKeyState = keyState;
        previousMouseState = mouseState;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSkyBlue);

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        var chunkMeshes = world.GetChunkMeshesDueRender();

        foreach (var mesh in chunkMeshes)
        {
            effect.Parameters["World"].SetValue(mesh.WorldMatrix);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    mesh.Vertices,
                    vertexOffset: 0,
                    mesh.VertexCount,
                    mesh.Indices,
                    indexOffset: 0,
                    mesh.TriangleCount
                );
            }
        }

        base.Draw(gameTime);

        spriteBatch.Begin(samplerState: SamplerState.PointWrap);

        if (CurrentGameState != GameState.Playing)
            menu.Draw(spriteBatch);

        DrawDebugUi(gameTime);
        spriteBatch.End();
    }
    public static void CenterMouse() => Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);

    protected override void UnloadContent()
    {
        buttonBackgroundTexture.Dispose();

        base.UnloadContent();
    }

    Menu CreateMenuForCurrentGameState()
    {
        if (CurrentGameState == GameState.Paused)
        {
            var resumeButton = new Button("Resume", font, buttonBackgroundTexture);
            resumeButton.OnClick += () => CurrentGameState = GameState.Playing;

            var settingsButton = new Button("Settings", font, buttonBackgroundTexture);
            settingsButton.OnClick += () => CurrentGameState = GameState.InSettingsMenu;

            var exitButton = new Button("Exit", font, buttonBackgroundTexture);
            exitButton.OnClick += Exit;

            return new Menu(
                screenCenter,
                new List<IUIElement>
                {
                    resumeButton,
                    settingsButton,
                    exitButton
                });
        }
        else if (CurrentGameState == GameState.InSettingsMenu)
        {
            var label = new Label("Settings", font);
            var temp = new Label("Nothing here yet", font);
            var backButton = new Button("Back", font, buttonBackgroundTexture);
            backButton.OnClick += () => CurrentGameState = GameState.Paused;

            return new Menu(
                screenCenter,
                new List<IUIElement>
                {
                    label,
                    temp,
                    backButton
                });
        }

        return null;
    }

    private void DrawDebugUi(GameTime gameTime)
    {
        var debugText = new StringBuilder();

        fps += ((1 / gameTime.ElapsedGameTime.TotalSeconds) - fps) * 0.1;
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
