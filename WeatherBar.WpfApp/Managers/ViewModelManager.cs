using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using WeatherBar.Application.Events.Interfaces;
using WeatherBar.Utils.Extensions;
using WeatherBar.WpfApp.ViewModel.Templates;

namespace WeatherBar.WpfApp.Managers
{
    public static class ViewModelManager
    {
        #region Fields

        private static readonly Dictionary<FrameworkElement, ViewModelBase> registeredViewModels = new Dictionary<FrameworkElement, ViewModelBase>();

        private static readonly Dictionary<ViewModelBase, Dictionary<string, MethodInfo>> handleMethodsInfoCache = new Dictionary<ViewModelBase, Dictionary<string, MethodInfo>>();

        private static readonly Dictionary<ViewModelBase, Dictionary<string, PropertyInfo>> propertiesInfoCache = new Dictionary<ViewModelBase, Dictionary<string, PropertyInfo>>();

        #endregion

        #region Public properties

        public static Dictionary<FrameworkElement, ViewModelBase> RegisteredViewModels => new Dictionary<FrameworkElement, ViewModelBase>(registeredViewModels);

        #endregion

        #region Public methods

        public static void Register<TViewModel>(TViewModel viewModel, FrameworkElement view) where TViewModel : ViewModelBase, new()
        {
            if (!registeredViewModels.ContainsKey(view))
            {
                registeredViewModels.Add(view, viewModel);
            }
            else
            {
                registeredViewModels[view] = viewModel;
            }

            CacheHandleMethodsInfo(viewModel);
            CachePropertiesInfo(viewModel);
        }

        public static void Unregister<TViewModel>(TViewModel viewModel, FrameworkElement view) where TViewModel : ViewModelBase, new()
        {
            registeredViewModels.Remove(view);
            handleMethodsInfoCache.Remove(viewModel);
            propertiesInfoCache.Remove(viewModel);
        }

        public static TViewModel GetRequired<TViewModel>(FrameworkElement view) where TViewModel : ViewModelBase
        {
            return Get<TViewModel>(view) ?? 
                throw new ArgumentException($"Could not find any matching {typeof(TViewModel)} ViewModel for {view.GetType()} View.");
        }

        public static TViewModel Get<TViewModel>(FrameworkElement view) where TViewModel : ViewModelBase
        {
            if (registeredViewModels.ContainsKey(view))
            {
                return (TViewModel)registeredViewModels[view];
            }

            return null;
        }

        public static void Notify<TContent>(IEvent<TContent> @event, ViewModelBase receiver, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            var shouldNotify = !propertiesInfoCache.TryGetValue(receiver, out Dictionary<string, PropertyInfo> propertiesDictionary) || !propertiesDictionary.TryGetValue(caller, out PropertyInfo property)
                || !property.GetValue(receiver).DeepCompare(@event.Content);

            if (shouldNotify)
            {
                try
                {
                    var handlerName = @event.GetType().Name;

                    handleMethodsInfoCache[receiver].TryGetValue(handlerName, out MethodInfo handler);

                    handler.Invoke(receiver, new[] { @event });
                }
                catch (ArgumentNullException)
                {
                    throw new InvalidOperationException($"ViewModel {receiver.GetType()} does not not support {@event.GetType()} handler call.");
                }
            }
        }

        public static void Notify(IEvent @event, ViewModelBase receiver, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            try
            {
                var handlerName = @event.GetType().Name;

                handleMethodsInfoCache[receiver].TryGetValue(handlerName, out MethodInfo handler);

                handler.Invoke(receiver, new[] { @event });
            }
            catch (ArgumentNullException)
            {
                throw new InvalidOperationException($"ViewModel {receiver.GetType()} does not not support {@event.GetType()} handler call.");
            }
        }

        #endregion

        #region Fields

        private static void CacheHandleMethodsInfo(ViewModelBase viewModel)
        {
            IEnumerable<MethodInfo> methods = viewModel.GetType().GetMethods().Where(x => x.IsVirtual && x.Name == nameof(IEventHandler<IEvent>.Handle) && x.GetParameters().Count() == 1);

            if (!handleMethodsInfoCache.ContainsKey(viewModel))
            {
                handleMethodsInfoCache.Add(viewModel, new Dictionary<string, MethodInfo>());
            }
            else
            {
                handleMethodsInfoCache[viewModel] = new Dictionary<string, MethodInfo>();
            }

            foreach (var method in methods)
            {
                var methodKey = method.GetParameters().First().ParameterType.Name;

                handleMethodsInfoCache[viewModel].Add(methodKey, method);
            }
        }

        private static void CachePropertiesInfo(ViewModelBase viewModel)
        {
            IEnumerable<PropertyInfo> properties = viewModel.GetType().GetProperties();

            if (!propertiesInfoCache.ContainsKey(viewModel))
            {
                propertiesInfoCache.Add(viewModel, new Dictionary<string, PropertyInfo>());
            }
            else
            {
                propertiesInfoCache[viewModel] = new Dictionary<string, PropertyInfo>();
            }

            foreach (var property in properties)
            {
                propertiesInfoCache[viewModel].Add(property.Name, property);
            }
        }

        #endregion
    }
}
