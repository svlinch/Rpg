public class BuffListChanged
{
    public bool IsLeftSide;
    public bool IsRemoved;
    public BuffModel Model;

    public BuffListChanged(bool isLeft, bool isRemoved, BuffModel model)
    {
        IsLeftSide = isLeft;
        IsRemoved = isRemoved;
        Model = model;
    }
}
