using System;
using WeatherBar.Core.Events.Enums;

namespace WeatherBar.Core.Events
{
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Properties

        public string CallerName { get; }

        public MessageType MessageType { get; }

        public object Message { get; }

        #endregion

        #region Constructors

        public MessageReceivedEventArgs(string callerName, MessageType messageType, object message)
        {
            CallerName = callerName;
            MessageType = messageType;
            Message = message;
        }

        #endregion
    }
}
