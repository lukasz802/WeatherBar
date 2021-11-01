using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WeatherBar.Controls.Templates;

namespace WeatherBar.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : SearchTextBoxBase
    {
        #region Fields

        private RoutedEventArgs args;

        private Popup popup;

        private ListBox itemList;

        #endregion

        #region Private properties

        protected override Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = SearchTextBoxControl.Template.FindName("PART_Popup", SearchTextBoxControl) as Popup;
                }

                return popup;
            }
        }

        protected override ListBox ItemList
        {
            get
            {
                if (itemList == null)
                {
                    itemList = SearchTextBoxControl.Template.FindName("PART_ItemList", SearchTextBoxControl) as ListBox;
                }

                return itemList;
            }
        }

        #endregion

        #region Public properties

        public override string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SearchTextBoxControl.TextChanged -= OnTextChanged;
                SetValue(TextProperty, value);
                SearchTextBoxControl.Text = value;
                SearchTextBoxControl.TextChanged += OnTextChanged;
            }
        }

        #endregion

        #region Constructors

        public SearchTextBox() : base()
        {
            InitializeComponent();
            this.GotFocus += SearchTxtBox_Focus;
            this.LostFocus += SearchTxtBox_Focus;
            this.KeyDown += SearchTxtBox_KeyDown;
            SearchTextBoxControl.LostFocus += SearchTextBoxControl_LostFocus;
            SearchTextBoxControl.GotFocus += SearchTextBoxControl_GotFocus;
            SearchButtonControl.Click += OnSearchClick;
            ClearButtonControl.Click += OnRemoveResultsClick;
            SearchTextBoxControl.TextChanged += OnTextChanged;
        }

        #endregion

        #region Private methods

        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (e.OriginalSource is ListBoxItem)
                    {
                        e.Handled = true;
                        Popup.IsOpen = false;

                        if (QueryResult != null && ItemList.SelectedItem != null)
                        {
                            QueryResult.Execute(ItemList.SelectedItem);
                            args = new RoutedEventArgs(QueryResultSelectedEvent);
                            RaiseEvent(args);
                            SearchTextBoxControl.Clear();
                        }
                    }
                    break;
                case Key.Up:
                case Key.Down:
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void ItemList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ItemList.SelectedItem != null
                && ((ListBoxItem)ItemList.ItemContainerGenerator.ContainerFromIndex(ItemList.SelectedIndex)).IsMouseOver)
            {
                Popup.IsOpen = false;
                e.Handled = true;

                if (QueryResult != null)
                {
                    QueryResult.Execute(ItemList.SelectedItem);
                    args = new RoutedEventArgs(QueryResultSelectedEvent);
                    RaiseEvent(args);
                    SearchTextBoxControl.Clear();
                }
            }
        }

        private void SearchTextBoxControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            DefaultSearchLabelControl.Visibility = ((TextBox)sender).Text != "" ? Visibility.Hidden : Visibility.Visible;
        }

        private void SearchTextBoxControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            SearchTextBoxControl.LostFocus -= SearchTextBoxControl_LostFocus;

            if (e.Key == Key.Down && ItemList.Items.Count > 0 && !(e.OriginalSource is ListBoxItem))
            {
                ItemList.Focus();
                ItemList.SelectedIndex = 0;
                ((ListBoxItem)ItemList.ItemContainerGenerator.ContainerFromIndex(ItemList.SelectedIndex)).Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Up && ItemList.SelectedIndex == 0)
            {
                SearchTextBoxControl.Focus();
                ItemList.UnselectAll();
            }
            else if (e.Key == Key.Down && ItemList.Items.Count > 0)
            {
                ItemList.SelectedIndex += 1;
                ((ListBoxItem)ItemList.ItemContainerGenerator.ContainerFromIndex(ItemList.SelectedIndex)).Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Up && ItemList.Items.Count > 0 && ItemList.SelectedItem != null)
            {
                ItemList.SelectedIndex -= 1;
                ((ListBoxItem)ItemList.ItemContainerGenerator.ContainerFromIndex(ItemList.SelectedIndex)).Focus();
                e.Handled = true;
            }

            SearchTextBoxControl.LostFocus += SearchTextBoxControl_LostFocus;
        }

        private void SearchTxtBox_Focus(object sender, RoutedEventArgs e)
        {
            DefaultSearchLabelControl.Foreground =
                !this.IsKeyboardFocusWithin ? (Brush)Application.Current.FindResource("UpdateInfoColorBrush") : Brushes.White;
        }

        private void ClearButtonControl_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBoxControl.Clear();
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }

            args = new RoutedEventArgs(SearchClickEvent);
            RaiseEvent(args);
            SearchTextBoxControl.Clear();
        }

        private void OnRemoveResultsClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            args = new RoutedEventArgs(RemoveResultsClickEvent);
            RaiseEvent(args);
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Text = SearchTextBoxControl.Text;
            Popup.IsOpen = false;
           
            EventDispatcher.Restart();

            args = new RoutedEventArgs(TextChangedEvent);
            RaiseEvent(args);
        }

        private void SearchTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSearchClick(sender, e);
            }
        }

        private void SearchTextBoxControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Popup != null && ItemsSource != null)
            {
                Popup.IsOpen = ItemsSource.GetEnumerator().MoveNext();
            }
        }

        private void SearchTextBoxControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Popup != null && !Popup.IsMouseOver)
            {
                Popup.IsOpen = false;
            }
        }

        #endregion
    }
}
