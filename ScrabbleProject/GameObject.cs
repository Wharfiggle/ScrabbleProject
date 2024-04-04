using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScrabbleProject;

public class GameObject
{
    string spritePath;
    Texture2D sprite;
    Vector2 pos;
    Vector2 size;
    Color color;
    bool centerOrigin;
    Game1 game;
    public bool loaded = false;

    public GameObject()
    {
        this.spritePath = "";
        this.pos = new Vector2(0, 0);
        this.size = new Vector2(-1, -1);
        this.color = Color.White;
        this.centerOrigin = false;
    }
    public GameObject(string spritePath, Vector2 pos)
    {
        this.spritePath = spritePath;
        this.pos = pos;
        this.size  = new Vector2(-1, -1);
        this.color = Color.White;
        this.centerOrigin = false;
    }
    public GameObject(string spritePath, Vector2 pos, bool centerOrigin, Vector2 size, Color color)
    {
        this.spritePath = spritePath;
        this.pos = pos;
        this.size = size;
        this.color = color;
        this.centerOrigin = centerOrigin;
    }

    public void SetGameReference(Game1 game)
    {
        this.game = game;
    }

    public virtual void LoadContent(ContentManager Content)
    {
        sprite = Content.Load<Texture2D>(spritePath);
        if(size.X == -1)
            size.X = sprite.Bounds.Width;
        else if(size.Y == -1)
            size.Y = sprite.Bounds.Height;
        loaded = true;
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        Vector2 drawPos = pos;
        if(centerOrigin)
            drawPos = new Vector2(pos.X - size.X / 2, pos.Y - size.Y / 2);
        _spriteBatch.Draw(sprite, new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)size.X, (int)size.Y), color);
    }
}