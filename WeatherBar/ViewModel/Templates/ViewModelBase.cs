using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WeatherBar.Core;
using WeatherBar.Core.Events.Args;
using WeatherBar.Core.Events.Enums;
using WeatherBar.Extensions;

namespace WeatherBar.ViewModel.Templates
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private readonly Dictionary<string, ViewModelMember> viewModelMembersMap = new Dictionary<string, ViewModelMember>();

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

        public bool ReceiveOnlyPublicChanges { get; set; }

        public bool SendOnlyPublicChanges { get; set; }

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
            PrepareMessageTypeMap();

            AutomaticallyApplyReceivedChanges = false;
            ReceiveOnlyPublicChanges = true;
            SendOnlyPublicChanges = true;
        }

        #endregion

        #region Public methods

        public void Notify([System.Runtime.CompilerServices.CallerMemberName] string caller = "", object message = null)
        {
            var getMessageTypeResult = viewModelMembersMap.TryGetValue(caller, out ViewModelMember viewModelMember);
            MessageType messageType;

            messageType = getMessageTypeResult ? viewModelMember.MessageType : MessageType.OtherInformationSend;

            if (SendOnlyPublicChanges && messageType.ToString().Contains("Private"))
            {
                return;
            }

            switch (messageType)
            {
                case MessageType.PublicPropertyChanged:
                    message = message ?? viewModelMember.GetValue();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
                    break;
                case MessageType.PrivateFieldChanged:
                case MessageType.PublicFieldChanged:
                case MessageType.PrivatePropertyChanged:
                    message = message ?? viewModelMember.GetValue();
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

        private void PrepareMessageTypeMap()
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                viewModelMembersMap.Add(property.Name, new ViewModelMember(property, this));
            }

            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                viewModelMembersMap.Add(field.Name, new ViewModelMember(field, this));
            }
        }

        private void ViewModelBase_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageType.PrivateFieldChanged:
                case MessageType.PrivatePropertyChanged:
                    if (!ReceiveOnlyPublicChanges)
                    {
                        TrySetNewValue(e);
                    }
                    break;
                case MessageType.PublicPropertyChanged:
                case MessageType.PublicFieldChanged:
                    TrySetNewValue(e);
                    break;
            }
        }

        private void TrySetNewValue(MessageReceivedEventArgs messageReceivedEventArgs)
        {
            var getMessageTypeResult = viewModelMembersMap.TryGetValue(messageReceivedEventArgs.CallerName, out ViewModelMember viewModelMember);

            if (getMessageTypeResult && !viewModelMember.GetValue().DeepCompare(messageReceivedEventArgs.Message))
            {
                viewModelMember.SetValue(messageReceivedEventArgs.Message);
            }
        }

        #endregion
    }
}
