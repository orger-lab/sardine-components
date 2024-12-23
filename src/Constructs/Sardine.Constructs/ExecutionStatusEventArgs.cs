namespace Sardine.Constructs
{
    public sealed class ExecutionStatusEventArgs : EventArgs
    {
        public bool IsExecuting { get; }

        public ExecutionStatusEventArgs(bool isExecuting)
        {
            IsExecuting = isExecuting;
        }
    }
}