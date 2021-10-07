using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WebApi.Model.Enums;
using WebApi.Model.Interfaces;

namespace WebApi.Model.DataTransferObjects
{
    public class DailyForecastTransferObject : IDailyData
    {
        #region Fields

        private readonly Dictionary<Language, string> dateDict = new Dictionary<Language, string>();

        private readonly Dictionary<Language, string> descriptionDict = new Dictionary<Language, string>();

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Propeties

        public int MaxTemp { get; private set; }

        public int MinTemp { get; private set; }

        public string Date
        {
            get
            {
                return dateDict.Count != 0 ? dateDict[Language] : null;
            }
        }

        public string Icon { get; private set; }

        public string Description 
        { 
            get
            {
                return descriptionDict.Count != 0 ? descriptionDict[Language] : null;
            }
        }

        public string DescriptionId { get; private set; }

        public Language Language { get; private set; }

        #endregion

        #region Construcors

        public DailyForecastTransferObject(double maxTemp, double minTemp, string icon, string description, string descriptionId, DayOfWeek weekDay, string date)
        {
            PrepareDictionaries(Language.Polish, description, descriptionId, weekDay, date);
            PrepareDictionaries(Language.English, description, descriptionId, weekDay, date);

            Language = Language.Polish;
            DescriptionId = descriptionId;
            MaxTemp = Convert.ToInt32(maxTemp);
            MinTemp = Convert.ToInt32(minTemp);
            Icon = icon;
        }

        #endregion

        #region Public methods

        public void ChangeLanguage(Language language)
        {
            Language = language;

            OnPropertyChanged("Description");
            OnPropertyChanged("Date");
        }

        #endregion

        #region Private methods

        private void PrepareDictionaries(Language language, string description, string descriptionId, DayOfWeek weekDay, string date)
        {
            var cultureName = new CultureInfo(language == Language.English ? "en-US" : "pl-PL");

            Match local = Regex.Match(date, @"(?<Month>(\d{1,2}))-(?<Day>(\d{1,2}))", RegexOptions.RightToLeft);

            dateDict.Add(language, cultureName.DateTimeFormat.GetAbbreviatedDayName(weekDay) + ", " +
                           int.Parse(local.Groups["Day"].Value).ToString() + " " + cultureName.DateTimeFormat.GetAbbreviatedMonthName(int.Parse(local.Groups["Month"].Value)));

            if (language == Language.Polish)
            {
                descriptionDict.Add(language, cultureName.DateTimeFormat.GetDayName(weekDay).First().ToString().ToUpper() +
                           cultureName.DateTimeFormat.GetDayName(weekDay).Substring(1) + ", " + description);
            }
            else
            {
                descriptionDict.Add(language, cultureName.DateTimeFormat.GetDayName(weekDay).First().ToString().ToUpper() +
                           cultureName.DateTimeFormat.GetDayName(weekDay).Substring(1) + ", " + Utils.GetDescriptionFromId(descriptionId));
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
