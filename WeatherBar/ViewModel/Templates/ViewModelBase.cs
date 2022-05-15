using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WeatherBar.Core;
using WeatherBar.Core.Events;
using WeatherBar.Core.Events.Enums;
using WeatherBar.Extensions;

namespace WeatherBar.ViewModel.Templates
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private bool automaticallyApplyReceivedChanges;

        #endregion

        #region Properties

        public bool AutomaticallyApplyReceivedChanges 
        {
            get => automaticallyApplyReceivedChanges;
            set
            {
                automaticallyApplyReceivedChanges = value;

                if (value)
                {
                    this.MessageReceived += ViewModelBase_MessageReceived;
                }
                else
                {
                    this.MessageReceived -= ViewModelBase_MessageReceived;
                }
            }
        }

        public bool IncludeOnlyPublicChanges { get; set; }

        #endregion

        #region Delegates

        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event MessageReceivedEventHandler MessageReceived;

        #endregion

        #region Constructors

        public ViewModelBase()
        {
            AutomaticallyApplyReceivedChanges = false;
            IncludeOnlyPublicChanges = true;
        }

        #endregion

        #region Public methods

        public void Notify([System.Runtime.CompilerServices.CallerMemberName] string caller = "", object message = null)
        {
            var messageType = GetMessageType(caller);

            switch (messageType)
            {
                case MessageType.PublicPropertyChanged:
                    message = message ?? GetType().GetProperty(caller)?.GetValue(this);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
                    break;
                case MessageType.PrivatePropertyChanged:
                    message = message ?? GetType().GetProperty(caller, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(this);
                    break;
                case MessageType.PublicFieldChanged:
                    message = message ?? GetType().GetField(caller)?.GetValue(this);
                    break;
                case MessageType.PrivateFieldChanged:
                    message = message ?? GetType().GetField(caller, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(this);
                    break;
            }

            var eventArg = new MessageReceivedEventArgs(caller, messageType, message);

            foreach (var viewModel in ViewModelManager.RegisteredViewModels.Where(x => x.Value != this).Select(x => x.Value))
            {
                viewModel.MessageReceived?.Invoke(this, eventArg);
            }
        }

        #endregion

        #region Private methods

        private MessageType GetMessageType(string caller)
        {
            var property = GetType().GetProperty(caller, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property != null)
            {
                return property.GetMethod.IsPublic ? MessageType.PublicPropertyChanged : MessageType.PrivatePropertyChanged;
            }

            var field = GetType().GetField(caller, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return field != null ? field.FieldType.IsPublic ? MessageType.PublicFieldChanged : MessageType.PrivateFieldChanged
                : MessageType.OtherInformationSend;
        }

        private void ViewModelBase_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageType.PrivateFieldChanged:
                    if (!IncludeOnlyPublicChanges)
                    {
                        TrySetFieldValue(e);
                    }
                    break;
                case MessageType.PublicFieldChanged:
                    TrySetFieldValue(e);
                    break;
                case MessageType.PrivatePropertyChanged:
                    if (!IncludeOnlyPublicChanges)
                    {
                        TrySetPropertyValue(e);
                    }
                    break;
                case MessageType.PublicPropertyChanged:
                    TrySetPropertyValue(e);
                    break;
            }
        }

        private void TrySetPropertyValue(MessageReceivedEventArgs messageReceivedEventArgs)
        {
            var property = this.GetType().GetProperty(messageReceivedEventArgs.CallerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property != null && !property.GetValue(this).DeepCompare(messageReceivedEventArgs.Message))
            {
                if (property.SetMethod != null)
                {
                    property.SetValue(this, messageReceivedEventArgs.Message);
                }
            }
        }

        private void TrySetFieldValue(MessageReceivedEventArgs messageReceivedEventArgs)
        {
            var field = this.GetType().GetField(messageReceivedEventArgs.CallerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null && field.GetValue(this).DeepCompare(messageReceivedEventArgs.Message))
            {
                field.SetValue(this, messageReceivedEventArgs.Message);
            }
        }

        #endregion
    }
}
