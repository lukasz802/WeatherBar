
namespace WebApi.Model.Interfaces
{
    public interface IDailyData : IWeatherData
    {
        int MaxTemp { get; }

        int MinTemp { get; }

        string Date { get; }

        string Description { get; }

        string DescriptionId { get; }

        string Icon { get; }
    }
}
