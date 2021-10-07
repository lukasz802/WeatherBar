using WebApi.Model.Enums;

namespace WebApi.Model.Interfaces
{
    public interface IMultiLanguage
    {
        #region Properties

        Language Language { get; }

        #endregion

        #region Methods

        void ChangeLanguage(Language language);

        #endregion
    }
}
