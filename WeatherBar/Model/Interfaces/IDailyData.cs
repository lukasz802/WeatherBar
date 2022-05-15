
namespace WeatherBar.Model.Interfaces
{
    public interface IDailyData : IMultiLanguage, IUnits, IClonable<IDailyData>
    {
        int MaxTemp { get; }

        int MinTemp { get; }

        string Date { get; }

        string Description { get; }

        string DescriptionId { get; }

        string Icon { get; }
    }
}
