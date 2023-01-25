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
        }

        public static void Unregister<TViewModel>(this TViewModel viewModel, FrameworkElement view) where TViewModel : ViewModelBase, new()
        {
            registeredViewModels.Remove(view);
        }

        public static TViewModel GetRequired<TViewModel>(FrameworkElement view) where TViewModel : ViewModelBase
        {
            if (registeredViewModels.ContainsKey(view))
            {
                return (TViewModel)registeredViewModels[view];
            }

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

        #endregion
    }
}
