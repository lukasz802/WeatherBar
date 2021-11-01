using System.ComponentModel;

namespace WeatherBar.Model.Interfaces
{
    public interface IDailyData : INotifyPropertyChanged, IMultiLanguage, IUnits
    {
        int MaxTemp { get; }

        int MinTemp { get; }

        string Date { get; }

        string Description { get; }

        string DescriptionId { get; }

        string Icon { get; }
    }
}
