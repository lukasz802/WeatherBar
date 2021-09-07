using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WeatherBar.Core;

namespace WeatherBar.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : UserControl
    {
        #region Fields

        private static bool isResultLoading = false;

        private RoutedEventArgs args;

        private Popup popup;

        private ListBox itemList;

        private EventDispatcher eventDispatcher;

        #endregion

        #region Events implementation

        public event RoutedEventHandler RemoveResultsClick
        {
            add { AddHandler(RemoveResultsClickEvent, value); }
            remove { RemoveHandler(RemoveResultsClickEvent, value); }
        }

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public event RoutedEventHandler SearchClick
        {
            add { AddHandler(SearchClickEvent, value); }
            remove { RemoveHandler(SearchClickEvent, value); }
        }

        public static readonly RoutedEvent SearchClickEvent =
            EventManager.RegisterRoutedEvent("SearchClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBox));

        public static readonly RoutedEvent RemoveResultsClickEvent =
            EventManager.RegisterRoutedEvent("RemoveResultsClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBox));

        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent("TextChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBox));

        #endregion

        #region Properties implementation

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SearchTextBox),
            new FrameworkPropertyMetadata(propertyChangedCallback: OnItemsSourceChanged));


        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register("Query", typeof(ICommand), typeof(SearchTextBox));

        public ICommand Query
        {
            get { return (ICommand)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        public static readonly DependencyProperty QueryParameterProperty =
            DependencyProperty.Register("QueryParameter", typeof(object), typeof(SearchTextBox));

        public object QueryParameter
        {
            get { return (object)GetValue(QueryParameterProperty); }
            set { SetValue(QueryParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SearchTextBox));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SearchTextBox));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty QueryResultProperty =
            DependencyProperty.Register("QueryResult", typeof(ICommand), typeof(SearchTextBox));

        public ICommand QueryResult
        {
            get { return (ICommand)GetValue(QueryResultProperty); }
            set { SetValue(QueryResultProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SearchTextBox));

        public string Text
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

        #region Private properties

        private Popup Popup
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

        private ListBox ItemList
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

        #region Constructors

        public SearchTextBox()
        {
            InitializeComponent();
            this.GotFocus += SearchTxtBox_Focus;
            this.LostFocus += SearchTxtBox_Focus;
            this.KeyDown += SearchTxtBox_KeyDown;
            this.Loaded += (s, e) => AutoRepositionPopupBehavior();
            SearchTextBoxControl.LostFocus += SearchTextBoxControl_LostFocus;
            SearchTextBoxControl.GotFocus += SearchTextBoxControl_GotFocus;
            SearchButtonControl.Click += OnSearchClick;
            ClearButtonControl.Click += OnRemoveResultsClick;
            SearchTextBoxControl.TextChanged += OnTextChanged;
            InitializeEventDispatcher();
        }

        #endregion

        #region Private methods

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SearchTextBox searchTextBox = (SearchTextBox)sender;

            if (searchTextBox.Popup != null && !isResultLoading)
            {
                searchTextBox.Popup.IsOpen = ((IEnumerable)e.NewValue).GetEnumerator().MoveNext();
            }

            isResultLoading = false;
        }

        private void InitializeEventDispatcher()
        {
            eventDispatcher = new EventDispatcher(() =>
            {
                if (Query != null)
                {
                    ICommand command = Query;

                    if (command.CanExecute(QueryParameter))
                    {
                        command.Execute(QueryParameter);
                    }
                }
            }, 400);
        }

        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is ListBoxItem && e.Key == Key.Enter)
            {
                Popup.IsOpen = false;
                e.Handled = true;

                if (QueryResult != null && ItemList.SelectedItem != null)
                {
                    QueryResult.Execute(ItemList.SelectedItem);
                    Query.Execute(string.Empty);
                }
            }
        }

        private void ItemList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Popup.IsOpen = false;
                e.Handled = true;

                if (QueryResult != null && ItemList.SelectedItem != null)
                {
                    QueryResult.Execute(ItemList.SelectedItem);
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
            isResultLoading = true;

            if (Command != null)
            {
                ICommand command = Command;

                if (command.CanExecute(CommandParameter))
                {
                    command.Execute(CommandParameter);
                }
            }

            args = new RoutedEventArgs(SearchClickEvent);
            RaiseEvent(args);
        }

        private void OnRemoveResultsClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Query.Execute(string.Empty);
            args = new RoutedEventArgs(RemoveResultsClickEvent);
            RaiseEvent(args);
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Text = SearchTextBoxControl.Text;
            Popup.IsOpen = false;

            eventDispatcher.Restart();

            args = new RoutedEventArgs(TextChangedEvent);
            RaiseEvent(args);
        }

        private void AutoRepositionPopupBehavior()
        {
            Window window = Window.GetWindow(SearchTextBoxControl);

            if (window != null || Popup == null)
            {
                window.LocationChanged += (s, t) =>
                {
                    var offset = Popup.HorizontalOffset;

                    Popup.HorizontalOffset = offset + 1;
                    Popup.HorizontalOffset = offset;
                };
            }

            Popup.PlacementTarget = SearchTextBoxControl;
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
            if (Popup != null)
            {
                Popup.IsOpen = false;
            }
        }

        #endregion
    }
}
