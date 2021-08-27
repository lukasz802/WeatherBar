namespace WebApi.Models.Interfaces
{
    public interface IDailyData : IWeatherData
    {
        int MaxTemp { get; set; }

        int MinTemp { get; set; }

        string Date { get; set; }

        string Description { get; set; }

        string Icon { get; set; }
    }
}
