using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScrabbleProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Vector2 windowSize = new Vector2(1280, 720);
    public List<GameObject> gameObjects = new List<GameObject>();
    private bool loaded = false;
    ScrabbleGame scrabble;
    public SpriteFont font;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
        gameObject.SetGameReference(this);
        if(loaded && !gameObject.loaded)
            gameObject.LoadContent(Content);
    }

    protected override void Initialize()
    {
        //If you have not used graphics yet, then using GraphicsDevice will crash the game. Calling ApplyChanges() prevents this.
        if(GraphicsDevice == null)
            _graphics.ApplyChanges();

        //Set window resolution
        _graphics.PreferredBackBufferWidth = (int)windowSize.X;
        _graphics.PreferredBackBufferHeight = (int)windowSize.Y;
        _graphics.ApplyChanges();

        scrabble = new ScrabbleGame(this);

        base.Initialize();
    }

    //load game assets like sprites and fonts
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        font = Content.Load<SpriteFont>("Fonts/CourierNew24");

        for(int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].LoadContent(Content);
        }
    }

    //tick
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        for(int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].Update(gameTime);
        }

        base.Update(gameTime);
    }

    //draw graphics
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        
        scrabble.Draw(gameTime, _spriteBatch);

        for(int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].Draw(gameTime, _spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
