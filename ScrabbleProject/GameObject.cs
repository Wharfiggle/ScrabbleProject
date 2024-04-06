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

    public GameObject(string spritePath = "", Vector2 pos = default, bool centerOrigin = false, Vector2 size = default, Color color = default)
    {
        this.spritePath = spritePath;
        this.pos = pos;
        this.centerOrigin = centerOrigin;
        this.size = size;
        this.color = color;
    }

    public void SetGameReference(Game1 game)
    {
        this.game = game;
    }

    public virtual void LoadContent(ContentManager Content)
    {
        sprite = Content.Load<Texture2D>(spritePath);
        if(size.X == 0)
            size.X = sprite.Bounds.Width;
        else if(size.Y == 0)
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