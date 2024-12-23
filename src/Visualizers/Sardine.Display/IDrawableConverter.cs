namespace Sardine.Display
{
    public interface IDrawableConverter<K,T> : IDrawableConverter<K>
    {
        IDrawable<K> Convert(T drawable);
        IDrawable<K> IDrawableConverter<K>.Convert(object drawable) => Convert((T)drawable);
        Type IDrawableConverter<K>.TypeOfDrawable => typeof(T);
    }

    public interface IDrawableConverter<K>
    {
        IDrawable<K> Convert(object drawable);
        Type TypeOfDrawable { get; }
    }
}
