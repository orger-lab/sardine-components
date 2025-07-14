using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class OnDCamErrorEventArgs : EventArgs
    {
        public HamamatsuError Error { get; }
        public string ErrorMessage { get; }
        public string Operation { get; }

        internal OnDCamErrorEventArgs(HamamatsuError error, string operation)
        {
            Error = error;
            ErrorMessage = error.ToString();
            Operation = operation;
        }

        public override string ToString()
        {
            return $"Error in {Operation}() - {ErrorMessage}";
        }
    }
}
