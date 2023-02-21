using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.WpfApp.Managers;

namespace WeatherBar.WpfApp.ViewModel.Templates
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private readonly Dictionary<string, PropertyInfo> propertiesInfoCache = new Dictionary<string, PropertyInfo>();

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ViewModelBase()
        {
            CachePropertiesInfo();
        }

        #endregion

        #region Public methods

        public void Notify<TContent>(IEvent<TContent> @event, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            Type eventType = @event.GetType();
            Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            foreach(var viewModel in ViewModelManager.RegisteredViewModels.Select(x => x.Value).Where(x => x != this && handlerType.IsAssignableFrom(x.GetType())).ToList())
            {
                ViewModelManager.Notify(@event, viewModel, caller);
                viewModel.Notify(new UpdatePropertiesEvent(viewModel));
            }

            NotifyPropertyChanged();
        }

        public void Notify(IEvent @event, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            if (@event.Source == this && @event is UpdatePropertiesEvent)
            {
                NotifyPropertyChanged();
                return;
            }

            Type eventType = @event.GetType();
            Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            foreach (var viewModel in ViewModelManager.RegisteredViewModels.Select(x => x.Value).Where(x => x != this && handlerType.IsAssignableFrom(x.GetType())).ToList())
            {
                ViewModelManager.Notify(@event, viewModel, caller);
            }

            NotifyPropertyChanged();
        }

        #endregion

        #region Private methods

        private void NotifyPropertyChanged()
        {
            foreach (var property in propertiesInfoCache)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.Key));
            }
        }

        private void CachePropertiesInfo()
        {
            IEnumerable<PropertyInfo> properties = GetType().GetProperties();

            foreach (var property in properties)
            {
                propertiesInfoCache.Add(property.Name, property);
            }
        }

        #endregion

        #region Private classes

        private class UpdatePropertiesEvent : IEvent
        {
            #region Properties

            public object Source { get; }

            #endregion

            #region Constructor

            public UpdatePropertiesEvent(ViewModelBase source)
            {
                Source = source;
            }

            #endregion
        }

        #endregion
    }
}
