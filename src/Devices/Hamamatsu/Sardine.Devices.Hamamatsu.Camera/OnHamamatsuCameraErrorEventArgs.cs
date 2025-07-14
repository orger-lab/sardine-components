using Sardine.Devices.Hamamatsu.Camera.API;

namespace Sardine.Devices.Hamamatsu.Camera
{
    public class OnHamamatsuCameraErrorEventArgs : EventArgs
    {
        public int ErrorCode { get; }
        public string ErrorMessage { get; }
        public string Operation { get; }
        internal OnHamamatsuCameraErrorEventArgs(HamamatsuError error, string operation)
        {
            ErrorCode = error;
            ErrorMessage = error.ToString();
            Operation = operation;
        }

        public override string ToString()
        {
            return $"Error in {Operation}() - {ErrorMessage}";
        }
    }
}
