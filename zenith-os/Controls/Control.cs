namespace zenithos.Controls
{
    public abstract class Control
    {
        public int x, y;
        public bool Visible = true;
        public abstract void Update(int pX,int pY);
    }
}
