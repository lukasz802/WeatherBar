using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WeatherBar.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SearchTextBox.xaml
    /// </summary>
    public partial class SearchTextBox : UserControl
    {
        #region Fields

        private RoutedEventArgs _args;

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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SearchTextBox));


        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register("Query", typeof(ICommand), typeof(SearchTextBox));

        public ICommand Query
        {
            get { return (ICommand)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
           DependencyProperty.Register("Command", typeof(ICommand), typeof(SearchTextBox));

        public static readonly DependencyProperty QueryParameterProperty =
            DependencyProperty.Register("QueryParameter", typeof(object), typeof(SearchTextBox));

        public object QueryParameter
        {
            get { return (object)GetValue(QueryParameterProperty); }
            set { SetValue(QueryParameterProperty, value); }
        }

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

        #region Constructors

        public SearchTextBox()
        {
            InitializeComponent();
            this.GotFocus += SearchTxtBox_Focus;
            this.LostFocus += SearchTxtBox_Focus;
            this.KeyDown += SearchTxtBox_KeyDown;
            SearchButtonControl.Click += OnSearchClick;
            ClearButtonControl.Click += OnRemoveResultsClick;
            SearchTextBoxControl.TextChanged += OnTextChanged;
        }

        #endregion

        #region Private methods

        private void SearchTextBoxControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                DefaultSearchLabelControl.Visibility = Visibility.Hidden;
            }
            else
            {
                DefaultSearchLabelControl.Visibility = Visibility.Visible;
            }
        }

        private void SearchTxtBox_Focus(object sender, RoutedEventArgs e)
        {
            if (this.IsKeyboardFocusWithin == false)
            {
                DefaultSearchLabelControl.Foreground = (Brush)Application.Current.FindResource("UpdateInfoColorBrush");
            }
            else
            {
                DefaultSearchLabelControl.Foreground = Brushes.White;
            }

        }

        private void ClearButtonControl_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBoxControl.Clear();
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (Command != null)
            {
                ICommand command = Command;

                if (command.CanExecute(CommandParameter))
                {
                    command.Execute(CommandParameter);
                }
            }

            _args = new RoutedEventArgs(SearchClickEvent);
            RaiseEvent(_args);
        }

        private void OnRemoveResultsClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _args = new RoutedEventArgs(RemoveResultsClickEvent);
            RaiseEvent(_args);
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Text = SearchTextBoxControl.Text;

            if (Query != null)
            {
                ICommand command = Query;

                if (command.CanExecute(QueryParameter))
                {
                    command.Execute(QueryParameter);
                }
            }

            _args = new RoutedEventArgs(TextChangedEvent);
            RaiseEvent(_args);
        }

        private void SearchTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSearchClick(sender, e);
            }
        }

        #endregion
    }
}
