using WeatherBar.Utils;
using WebApi.Model.Enums;

namespace WeatherBar.Model.DataTransferObjects
{
    public class WeatherDescriptionTransferObject
    {
        #region Fields

        private readonly string polishDescription;

        private readonly string englishDescription;

        #endregion

        #region Properties

        public string Description { get; private set; }

        #endregion

        #region Constructors

        public WeatherDescriptionTransferObject(string polishDescription, string descriptionId)
        {
            this.polishDescription = polishDescription;
            englishDescription = ApplicationUtils.GetDescriptionFromId(descriptionId);
            Description = this.polishDescription;
        }

        #endregion

        #region Public methods

        public void ChangeDescription(Language language)
        {
            Description = language == Language.English ? englishDescription : polishDescription;
        }

        #endregion
    }
}
