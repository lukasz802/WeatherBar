using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WeatherBar.Core;
using WeatherBar.Core.Events.Args;
using WeatherBar.Core.Events.Enums;
using WeatherBar.Extensions;

namespace WeatherBar.ViewModel.Templates
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private readonly ConcurrentDictionary<string, ReflectionProperty> reflectionPropertyCache = new ConcurrentDictionary<string, ReflectionProperty>();

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
            CacheReflectionPropertyData();

            AutomaticallyApplyReceivedChanges = false;
            ReceiveOnlyPublicChanges = true;
            SendOnlyPublicChanges = true;
        }

        #endregion

        #region Public methods

        public void Notify([System.Runtime.CompilerServices.CallerMemberName] string caller = "", object message = null)
        {
            var getMessageTypeResult = reflectionPropertyCache.TryGetValue(caller, out ReflectionProperty viewModelMember);
            var messageType = getMessageTypeResult ? viewModelMember.MessageType : MessageType.OtherInformationSend;

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

            Task.Run(() =>
            {
                foreach (var viewModel in ViewModelManager.RegisteredViewModels.Where(x => x.Value != this).Select(x => x.Value).ToList())
                {
                    viewModel.MessageReceived?.Invoke(this, eventArg);
                }
            });
        }

        #endregion

        #region Private methods

        private void CacheReflectionPropertyData()
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                reflectionPropertyCache.TryAdd(property.Name, new ReflectionProperty(property, this));
            }

            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                reflectionPropertyCache.TryAdd(field.Name, new ReflectionProperty(field, this));
            }
        }

        private void ViewModelBase_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Task.Run(() =>
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
            });
        }

        private void TrySetNewValue(MessageReceivedEventArgs messageReceivedEventArgs)
        {
            var getMessageTypeResult = reflectionPropertyCache.TryGetValue(messageReceivedEventArgs.CallerName, out ReflectionProperty reflectionProperty);

            if (getMessageTypeResult && !reflectionProperty.GetValue().DeepCompare(messageReceivedEventArgs.Message))
            {
                reflectionProperty.SetValue(messageReceivedEventArgs.Message);
            }
        }

        #endregion
    }
}
