using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WeatherBar.Controls.Enums;
using WeatherBar.Controls.Templates;
using WeatherBar.Extensions;
using WeatherBar.Utils;

namespace WeatherBar.Controls.UserControls
{
    public partial class CityPopupTextBox : SearchTextBoxBase
    {
        #region Fields

        private const double defaultResultHeight = 46;

        private static double defaultPopupHeight;

        private RoutedEventArgs args;

        private Popup popup;

        private ListBox itemList;

        private TextBox searchTextBoxControl;

        private Label defaultSearchLabelControl;

        private Label resultsLabelControl;

        private Grid backGridControl;

        private Grid searchGridControl;

        #endregion

        #region Properties implementation

        public override IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public new static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CityPopupTextBox),
            new FrameworkPropertyMetadata(propertyChangedCallback: OnItemsSourceChanged));

        public string CityName
        {
            get { return (string)GetValue(CityNameProperty); }
            set { SetValue(CityNameProperty, value); }
        }

        public static readonly DependencyProperty CityNameProperty =
            DependencyProperty.Register("CityName", typeof(string), typeof(CityPopupTextBox));

        public string Latitude
        {
            get { return (string)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }

        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(string), typeof(CityPopupTextBox));

        public string Longtitude
        {
            get { return (string)GetValue(LongtitudeProperty); }
            set { SetValue(LongtitudeProperty, value); }
        }

        public static readonly DependencyProperty LongtitudeProperty =
            DependencyProperty.Register("Longtitude", typeof(string), typeof(CityPopupTextBox));

        public string CityId
        {
            get { return (string)GetValue(CityIdProperty); }
            set { SetValue(CityIdProperty, value); }
        }

        public static readonly DependencyProperty CityIdProperty =
            DependencyProperty.Register("CityId", typeof(string), typeof(CityPopupTextBox));

        public string Country
        {
            get { return (string)GetValue(CountryProperty); }
            set { SetValue(CountryProperty, value); }
        }

        public static readonly DependencyProperty CountryProperty =
            DependencyProperty.Register("Country", typeof(string), typeof(CityPopupTextBox));

        #endregion

        #region Private properties

        private TextBox SearchTextBoxControl
        {
            get
            {
                if (searchTextBoxControl == null)
                {
                    searchTextBoxControl = (TextBox)Popup.FindName("SearchTextBoxControl");
                }

                return searchTextBoxControl;
            }
        }

        private Label DefaultSearchLabelControl
        {
            get
            {
                if (defaultSearchLabelControl == null)
                {
                    defaultSearchLabelControl = (Label)Popup.FindName("DefaultSearchLabelControl");
                }

                return defaultSearchLabelControl;
            }
        }

        private Label ResultsLabelControl
        {
            get
            {
                if (resultsLabelControl == null)
                {
                    resultsLabelControl = (Label)Popup.FindName("ResultsLabelControl");
                }

                return resultsLabelControl;
            }
        }

        private Grid BackGridControl
        {
            get
            {
                if (backGridControl == null)
                {
                    backGridControl = (FrameworkElement)Popup.FindName("BackGrid") as Grid;
                }

                return backGridControl;
            }
        }

        private Grid SearchGridControl
        {
            get
            {
                if (searchGridControl == null)
                {
                    searchGridControl = (FrameworkElement)Popup.FindName("SearchGrid") as Grid;
                }

                return searchGridControl;
            }
        }

        protected override Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = CityPopupControlButton.Template.FindName("Popup", CityPopupControlButton) as Popup;
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
                    itemList = CityPopupControlButton.Template.FindName("PART_ItemList", CityPopupControlButton) as ListBox;
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

        public CityPopupTextBox() : base()
        {
            InitializeComponent();
            CityPopupControlButton.LostFocus += CityPopupControlButton_LostFocus;
            Loaded += (s, e) =>
            {
                this.QueryStatusChanged += CityPopupTextBox_QueryStatusChanged;
                SearchTextBoxControl.TextChanged += OnTextChanged;
                SearchTextBoxControl.GotFocus += SearchTextBoxControl_GotFocus;
                SearchTextBoxControl.LostFocus += SearchTextBoxControl_LostFocus;
                CityPopupControlButton.Click += CityPopupControlButton_Click;
                CityPopupControlButton.PreviewKeyDown += SearchTextBoxControl_PreviewKeyDown;
                CityPopupControlButton.PreviewMouseDown += CityPopupControlButton_PreviewMouseDown;
                ItemList.PreviewMouseDown += ItemList_PreviewMouseDown;
            };
        }

        #endregion

        #region Private methods

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CityPopupTextBox cityPopupTextBox = (CityPopupTextBox)sender;

            if (cityPopupTextBox.ItemsSource != null)
            {
                if (((IEnumerable)e.NewValue).GetEnumerator().MoveNext())
                {
                    var multiplier = ((IEnumerable)e.NewValue).Count() <= 3 ? ((IEnumerable)e.NewValue).Count() : 3;

                    cityPopupTextBox.Popup.Height = defaultPopupHeight - defaultResultHeight + multiplier * defaultResultHeight;
                    cityPopupTextBox.ItemList.Visibility = Visibility.Visible;
                }
                else
                {
                    cityPopupTextBox.Popup.Height = defaultPopupHeight;
                    cityPopupTextBox.ResultsLabelControl.Content = Application.Current.Resources["NoResults"];
                }
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
                }
            }
        }

        private void CityPopupTextBox_QueryStatusChanged(object sender, RoutedPropertyChangedEventArgs<QueryStatus> e)
        {
            if (e.NewValue == QueryStatus.InProgress)
            {
                Popup.Height = defaultPopupHeight;
                ItemList.Visibility = Visibility.Collapsed;
                ResultsLabelControl.Content = Application.Current.Resources["Searching"];
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            DefaultSearchLabelControl.Visibility = ((TextBox)sender).Text != "" ? Visibility.Hidden : Visibility.Visible;
            e.Handled = true;
            Text = SearchTextBoxControl.Text;

            EventDispatcher.Restart();

            args = new RoutedEventArgs(TextChangedEvent);
            RaiseEvent(args);
        }

        private void CityPopupControlButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)Popup.FindName("BackGrid")).IsMouseOver || (ItemList.IsMouseOver && ItemList.SelectedItem != null))
            {
                Popup.IsOpen = false;
                SearchTextBoxControl.Clear();
            }
            else if (Popup != null && Popup.IsOpen && !(SearchTextBoxControl.IsMouseOver || ItemList.IsMouseOver))
            {;
                e.Handled = true;
            }
        }

        private void CityPopupControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (Popup != null && !Popup.IsOpen)
            {
                Resources["DefaultListBoxItemWidth"] = ActualWidth - 55;
                Popup.Width = ItemList.Width = ActualWidth;
                Popup.Height = defaultPopupHeight = ActualHeight + 16;
                Popup.VerticalOffset = -ActualHeight;
                Popup.IsOpen = true;
            }
        }

        private void CityPopupControlButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Popup != null && !IsMouseOver)
            {
                Popup.IsOpen = false;
                SearchTextBoxControl.Clear();
            }
        }

        private void SearchTextBoxControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Tab)
            {
                PreviewEscapeTabKeyDown(e);
            }
            else if (e.Key == Key.Up || e.Key == Key.Down)
            {
                PreviewUpDownKeyDown(e);
            }
            else if (e.Key == Key.Enter && ItemList.SelectedItem != null)
            {
                Popup.IsOpen = false;
                QueryResult?.Execute(ItemList.SelectedItem);
            }
        }

        private void PreviewEscapeTabKeyDown(KeyEventArgs e)
        {
            Popup.IsOpen = false;
            SearchTextBoxControl.Clear();

            if (e.Key == Key.Tab)
            {
                Frame parent = GlobalUtils.FindVisualParent<Frame>(this);
                Selector child = GlobalUtils.FindVisualChildren<Selector>(parent).FirstOrDefault();

                if (child != null)
                {
                    parent.Focusable = true;
                    child.Focus();
                    parent.Focusable = false;
                }
            }
        }

        private void PreviewUpDownKeyDown(KeyEventArgs e)
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

        private void SearchTextBoxControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var brush = (SolidColorBrush)Application.Current.Resources["BorderBackgroundColorBrush"];

            SearchGridControl.Visibility = Visibility.Hidden;
            BackGridControl.Visibility = Visibility.Visible;
            DefaultSearchLabelControl.Foreground = new SolidColorBrush()
            {
                Color = brush.Color,
                Opacity = 0.5
            };
        }

        private void SearchTextBoxControl_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchGridControl.Visibility = Visibility.Visible;
            BackGridControl.Visibility = Visibility.Hidden;
            DefaultSearchLabelControl.Foreground = (SolidColorBrush)Application.Current.Resources["BorderBackgroundColorBrush"];
        }

        #endregion
    }
}