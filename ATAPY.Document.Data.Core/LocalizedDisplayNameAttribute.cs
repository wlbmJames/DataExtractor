using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;

namespace ATAPY.Document.Data.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private PropertyInfo _nameProperty;
        private Type _resourceType;


        public LocalizedDisplayNameAttribute(string displayNameKey)
            : base(displayNameKey)
        {

        }

        public Type ResourceType
        {
            get
            {
                return _resourceType;
            }
            set
            {
                _resourceType = value;
                _nameProperty = _resourceType.GetProperty(base.DisplayName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        public override string DisplayName
        {
            get
            {
                if (_nameProperty == null) {
                    return base.DisplayName;
                }

                return (string)_nameProperty.GetValue(_nameProperty.DeclaringType, null);
            }
        }

    }
    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private PropertyInfo _nameProperty;
        private Type _resourceType;


        public LocalizedDescriptionAttribute(string description)
            : base(description)
        {

        }

        public Type ResourceType
        {
            get
            {
                return _resourceType;
            }
            set
            {
                _resourceType = value;
                _nameProperty = _resourceType.GetProperty(base.Description, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        public override string Description
        {
            get
            {
                if (_nameProperty == null) {
                    return base.Description;
                }

                return (string)_nameProperty.GetValue(_nameProperty.DeclaringType, null);
            }
        }

    }

    public class EnumTypeConverter : EnumConverter
    {
        private Type enumType;
        public EnumTypeConverter(Type type) : base(type)
        {
            enumType = type;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                   object value, Type destType)
        {
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, value));
            DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                        typeof(DescriptionAttribute));
            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
                                           object value)
        {
            foreach (FieldInfo fi in enumType.GetFields()) {
                DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                            typeof(DescriptionAttribute));
                if ((dna != null) && ((string)value == dna.Description))
                    return Enum.Parse(enumType, fi.Name);
            }
            return Enum.Parse(enumType, (string)value);
        }
    }
}
