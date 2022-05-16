using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WeatherBar.Controls.Enums;
using WeatherBar.Core.Events;

namespace WeatherBar.Controls.Templates
{
    public class SearchTextBoxBase : UserControl
    {
        #region Fields

        private EventDispatcher eventDispatcher;

        private QueryStatus queryStatus;

        #endregion

        #region Events implementation

        public event RoutedEventHandler SearchClick
        {
            add { AddHandler(SearchClickEvent, value); }
            remove { RemoveHandler(SearchClickEvent, value); }
        }

        public static readonly RoutedEvent SearchClickEvent =
            EventManager.RegisterRoutedEvent("SearchClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBoxBase));

        public event RoutedEventHandler QueryResultSelected
        {
            add { AddHandler(QueryResultSelectedEvent, value); }
            remove { RemoveHandler(QueryResultSelectedEvent, value); }
        }

        public static readonly RoutedEvent QueryResultSelectedEvent =
            EventManager.RegisterRoutedEvent("QueryResultSelected",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBoxBase));

        public event RoutedEventHandler RemoveResultsClick
        {
            add { AddHandler(RemoveResultsClickEvent, value); }
            remove { RemoveHandler(RemoveResultsClickEvent, value); }
        }

        public static readonly RoutedEvent RemoveResultsClickEvent =
            EventManager.RegisterRoutedEvent("RemoveResultsClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBoxBase));

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent("TextChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchTextBoxBase));

        #endregion

        #region Properties implementation

        public virtual IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SearchTextBoxBase),
            new FrameworkPropertyMetadata(propertyChangedCallback: OnItemsSourceChanged));

        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register("Query", typeof(ICommand), typeof(SearchTextBoxBase));

        public ICommand Query
        {
            get { return (ICommand)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        public static readonly DependencyProperty QueryParameterProperty =
            DependencyProperty.Register("QueryParameter", typeof(object), typeof(SearchTextBoxBase));

        public object QueryParameter
        {
            get { return (object)GetValue(QueryParameterProperty); }
            set { SetValue(QueryParameterProperty, value); }
        }

        public static readonly DependencyProperty QueryResultProperty =
            DependencyProperty.Register("QueryResult", typeof(ICommand), typeof(SearchTextBoxBase));

        public ICommand QueryResult
        {
            get { return (ICommand)GetValue(QueryResultProperty); }
            set { SetValue(QueryResultProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SearchTextBoxBase));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SearchTextBoxBase));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SearchTextBoxBase));

        public virtual string Text { get; set; }

        #endregion

        #region Private events

        protected event RoutedPropertyChangedEventHandler<QueryStatus> QueryStatusChanged;

        #endregion

        #region Private properties

        protected virtual Popup Popup { get; }

        protected virtual ListBox ItemList { get; }

        protected EventDispatcher EventDispatcher => eventDispatcher;

        protected QueryStatus QueryStatus 
        { 
            get
            {
                return queryStatus;
            }
            private set
            {
                if (queryStatus != value)
                {
                    var arg = new RoutedPropertyChangedEventArgs<QueryStatus>(QueryStatus, value);

                    queryStatus = value;
                    OnQueryStatusChanged(this, arg);
                }
            }
        }

        #endregion

        #region Constructors

        public SearchTextBoxBase()
        {
            InitializeEventDispatcher();
            this.TextChanged += (s ,e) => QueryStatus = QueryStatus.InProgress;
        }

        #endregion

        #region Private methods

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SearchTextBoxBase searchTextBoxBase = (SearchTextBoxBase)sender;

            if (searchTextBoxBase.Popup != null && searchTextBoxBase.ItemsSource != null)
            {
                searchTextBoxBase.Popup.IsOpen = ((IEnumerable)e.NewValue).GetEnumerator().MoveNext();
            }
        }

        private void OnQueryStatusChanged(object sender, RoutedPropertyChangedEventArgs<QueryStatus> e)
        {
            QueryStatusChanged?.Invoke(this, e);
        }

        private void InitializeEventDispatcher()
        {
            eventDispatcher = new EventDispatcher(() =>
            {
                if (Query != null && Query.CanExecute(QueryParameter))
                {
                    Query.Execute(QueryParameter);
                    QueryStatus = QueryStatus.Finished;
                }

                QueryStatus = QueryStatus.Pending;
            }, 300);
        }

        #endregion
    }
}
