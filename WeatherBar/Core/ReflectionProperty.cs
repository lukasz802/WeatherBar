using System.Reflection;
using WeatherBar.Core.Events.Enums;

namespace WeatherBar.Core
{
    public class ReflectionProperty
    {
        #region Private fields

        private readonly PropertyInfo property;

        private readonly FieldInfo field;

        private readonly object referenceObject;

        #endregion

        #region Properties

        public string Name { get; }

        public MessageType MessageType { get; }

        #endregion

        #region Constructors

        public ReflectionProperty(PropertyInfo property, object refObj)
        {
            this.property = property;
            this.referenceObject = refObj;
            MessageType = GetMessageType(property);
            Name = property.Name;
        }

        public ReflectionProperty(FieldInfo field, object refObj)
        {
            this.field = field;
            this.referenceObject = refObj;
            MessageType = GetMessageType(field);
            Name = field.Name;
        }

        #endregion

        #region Public methods

        public void SetValue(object value)
        {
            if (property == null)
            {
                field.SetValue(referenceObject, value);
                return;
            }

            if (property.SetMethod != null)
            {
                property.SetValue(referenceObject, value);
            }
        }

        public object GetValue()
        {
            if (property == null)
            {
                return field.GetValue(referenceObject);
            }

            return property.GetValue(referenceObject);
        }

        #endregion

        #region Private methods

        private MessageType GetMessageType(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsPublic ? MessageType.PublicPropertyChanged : MessageType.PrivatePropertyChanged;
        }

        private MessageType GetMessageType(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType.IsPublic ? MessageType.PublicFieldChanged : MessageType.PrivateFieldChanged;
        }

        #endregion
    }
}
