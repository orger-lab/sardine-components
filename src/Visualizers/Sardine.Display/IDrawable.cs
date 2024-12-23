namespace Sardine.Display
{
    public interface IDrawable<K>
    {
        bool IsAlive { get; }
        void Draw(K canvas, ref object? displayData, uint canvasWidth, uint canvasHeight, object displayOptions, Type displayOptionsType);
    }
}
