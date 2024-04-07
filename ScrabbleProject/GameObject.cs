using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScrabbleProject;

public class GameObject
{
    string spritePath;
    Vector2 pos;
    Vector2 size;
    bool centerOrigin;
    protected Vector2 drawPos;
    public Texture2D sprite;
    public Game1 game;
    public bool loaded = false;
    public Color color;
    private bool hovered = false;
    private bool clicked = false;

    //constructor
    public GameObject(string spritePath = "", Vector2 pos = default, bool centerOrigin = false, Vector2 size = default, Color color = default)
    {
        this.spritePath = spritePath;
        this.pos = new Vector2((int)pos.X, (int)pos.Y);
        this.centerOrigin = centerOrigin;
        SetSize(size);
        this.color = color;
    }

    //set reference to game
    public virtual void SetGameReference(Game1 game)
    {
        this.game = game;
    }

    //size setter and getter
    public void SetPos(Vector2 pos)
    {
        this.pos = new Vector2((int)pos.X, (int)pos.Y);
        UpdateDrawPos();
    }
    public Vector2 GetPos() {return pos;}

    //centerOrigin setter and getter
    public void SetCenterOrigin(bool centerOrigin)
    {
        this.centerOrigin = centerOrigin;
        UpdateDrawPos();
    }
    public bool GetCenterOrigin() {return centerOrigin;}
    
    //size setter and getter
    public void SetSize(Vector2 size)
    {
        this.size = new Vector2((int)size.X, (int)size.Y);
        UpdateDrawPos();
    }
    public Vector2 GetSize() {return size;}

    //drawPos is dependent on size, pos, and centerOrigin, so it must be updated whenever those are
    private void UpdateDrawPos()
    {
        if(centerOrigin)
            drawPos = new Vector2(pos.X - size.X / 2, pos.Y - size.Y / 2);
        else
            drawPos = pos;
    }

    //called during game's LoadContent
    public virtual void LoadContent(ContentManager Content)
    {
        if(spritePath != "")
            sprite = Content.Load<Texture2D>(spritePath);
        else
        {
            if(size.X == 0)
                size.X = sprite.Bounds.Width;
            else if(size.Y == 0)
                size.Y = sprite.Bounds.Height;
        }
        loaded = true;
    }

    //called during game's Update
    public virtual void Update(GameTime gameTime)
    {
        //sets hovered to true if the mouse is hovered over this GameObject, false otherwise.
        Vector2 mp = game.GetMousePos();
        hovered = mp.X >= drawPos.X && mp.Y >= drawPos.Y && mp.X < drawPos.X + size.X && mp.Y < drawPos.Y + size.Y;

        //sets clicked to true if hovered is true and the left mouse button was just clicked, false otherwise.
        clicked = hovered && game.GetMousePressed();
    }

    public bool isHovered() { return hovered; }
    public bool isClicked() { return clicked; }

    //called during game's Draw
    public virtual void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        if(spritePath != "")
            _spriteBatch.Draw(sprite, new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)size.X, (int)size.Y), color);
    }
}