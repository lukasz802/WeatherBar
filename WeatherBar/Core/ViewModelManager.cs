using System;
using System.Collections.Generic;
using System.Windows;
using WeatherBar.ViewModel.Templates;

namespace WeatherBar.Core
{
    public static class ViewModelManager
    {
        #region Fields

        private static readonly Dictionary<FrameworkElement, ViewModelBase> registeredViewModels = new Dictionary<FrameworkElement, ViewModelBase>();

        #endregion

        #region Public properties

        public static Dictionary<FrameworkElement, ViewModelBase> RegisteredViewModels => registeredViewModels;

        #endregion

        #region Public methods

        public static TViewModel CreateViewModel<TViewModel>(FrameworkElement view) where TViewModel : ViewModelBase, new()
        {
            if (registeredViewModels.ContainsKey(view))
            {
                return (TViewModel)registeredViewModels[view];
            }

            var instance = new TViewModel();

            if (view.IsLoaded)
            {
                view.DataContext = instance;
            }
            else
            {
                view.Loaded += (s, e) => view.DataContext = instance;
            }

            view.Unloaded += (s, e) => registeredViewModels.Remove(view);
            registeredViewModels.Add(view, instance);

            return instance;
        }

        public static TViewModel GetRequiredViewModel<TViewModel>(FrameworkElement view) where TViewModel : ViewModelBase
        {
            if (registeredViewModels.ContainsKey(view))
            {
                return (TViewModel)registeredViewModels[view];
            }

            throw new ArgumentException($"Could not find any matching {typeof(TViewModel)} ViewModel for {view.GetType()} View.");
        }

        #endregion
    }
}
