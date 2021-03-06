#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Core\XmlExtension.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2015, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace CK.Core
{
    /// <summary>
    /// Extension methods for <see cref="XmlReader"/> and <see cref="XElement"/>
    /// classes.
    /// </summary>
    public static class XmlExtension
    {
        #region XmlReader
        /// <summary>
        /// Little helper that only increases source code readability: it calls <see cref="XmlReader.ReadEndElement"/>
        /// that checks the name of the closing element. This "helper" forces the developper to explicitely
        /// write this name.
        /// </summary>
        /// <param name="this">This <see cref="XmlReader"/>.</param>
        /// <param name="name">Name of the closing element.</param>
        static public void ReadEndElement( this XmlReader @this, string name )
        {
            if( @this.NodeType != XmlNodeType.EndElement || @this.Name != name )
            {
                throw new XmlException( String.Format( R.ExpectedXmlEndElement, name ) );
            }
            @this.ReadEndElement();
        }

        /// <summary>
        /// Gets a boolean attribute by name.
        /// </summary>
        /// <param name="this">This <see cref="XmlReader"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public bool GetAttributeBoolean( this XmlReader @this, string name, bool defaultValue )
        {
            string s = @this.GetAttribute( name );
            return s != null ? XmlConvert.ToBoolean( s ) : defaultValue;
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> attribute by name. It uses <see cref="XmlDateTimeSerializationMode.RoundtripKind"/>.
        /// </summary>
        /// <param name="this">This <see cref="XmlReader"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public DateTime GetAttributeDateTime( this XmlReader @this, string name, DateTime defaultValue )
        {
            string s = @this.GetAttribute( name );
            return s != null ? XmlConvert.ToDateTime( s, XmlDateTimeSerializationMode.RoundtripKind ) : defaultValue;
        }

        /// <summary>
        /// Gets a <see cref="Version"/> attribute by name.
        /// </summary>
        /// <param name="this">This <see cref="XmlReader"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public Version GetAttributeVersion( this XmlReader @this, string name, Version defaultValue )
        {
            string s = @this.GetAttribute( name );
            return s != null ? new Version( s ) : defaultValue;
        }

        /// <summary>
        /// Gets an <see cref="Int32"/> attribute by name.
        /// </summary>
        /// <param name="r">This <see cref="XmlReader"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public int GetAttributeInt( this XmlReader r, string name, int defaultValue )
        {
            string s = r.GetAttribute( name );
            int i;
            if( s != null && int.TryParse( s, out i ) ) return i;
            return defaultValue;
        }

        /// <summary>
        /// Gets an enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum. There is no way (in c#) to constraint the type to Enum - nor to Delegate, this is why 
        /// the constraint restricts only the type to be a value type.</typeparam>
        /// <param name="this">This <see cref="XmlReader"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist or can not be parsed.</param>
        /// <returns>The parsed value or the default value.</returns>
        static public T GetAttributeEnum<T>( this XmlReader @this, string name, T defaultValue ) where T : struct
        {
            T result;
            string s = @this.GetAttribute( name );
            if( s == null || !Enum.TryParse( s, out result ) ) result = defaultValue;
            return result;
        }

        #endregion

        #region Xml.Linq

        /// <summary>
        /// Gets line and column information (if it exists) as a string from any <see cref="XObject"/> (such as <see cref="XAttribute"/> or <see cref="XElement"/>).
        /// </summary>
        /// <param name="this">This <see cref="IXmlLineInfo"/>.</param>
        /// <param name="format">Default format is "- @Line,Column".</param>
        /// <param name="noLineInformation">Defaults to a null string when <see cref="IXmlLineInfo.HasLineInfo()"/> is false.</param>
        /// <returns>A string based on <paramref name="format"/> or <paramref name="noLineInformation"/>.</returns>
        static public string GetLineColumString( this IXmlLineInfo @this, string format = "- @{0},{1}", string noLineInformation = null )
        {
            if( @this.HasLineInfo() ) return String.Format( format, @this.LineNumber, @this.LinePosition );
            return noLineInformation;
        }

        /// <summary>
        /// Gets the attribute by its name or throws an <see cref="XmlException"/> if it does not exist.
        /// </summary>
        /// <param name="this">This <see cref="XElement"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        static public XAttribute AttributeRequired( this XElement @this, XName name )
        {
            XAttribute a = @this.Attribute( name );
            if( a == null ) throw new XmlException( String.Format( R.ExpectedXmlAttribute, name ) + @this.GetLineColumString() );
            return a;
        }

        /// <summary>
        /// Gets a string attribute by name.
        /// </summary>
        /// <param name="this">This <see cref="XElement"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public string GetAttribute( this XElement @this, XName name, string defaultValue )
        {
            XAttribute a = @this.Attribute( name );
            return a != null ? a.Value : defaultValue;
        }

        /// <summary>
        /// Gets a boolean attribute by name.
        /// </summary>
        /// <param name="this">This <see cref="XElement"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public bool GetAttributeBoolean( this XElement @this, XName name, bool defaultValue )
        {
            XAttribute a = @this.Attribute( name );
            if( a == null ) return defaultValue;
            var v = a.Value;
            if( v == "0" || StringComparer.InvariantCultureIgnoreCase.Equals( v, "false" ) ) return false;
            if( v == "1" || StringComparer.InvariantCultureIgnoreCase.Equals( v, "true" ) ) return true;
            throw new FormatException( "Boolean value expected: false, true, 0 or 1 (case insensitive)." );
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> attribute by name. It uses <see cref="XmlDateTimeSerializationMode.RoundtripKind"/>.
        /// </summary>
        /// <param name="this">This <see cref="XElement"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public DateTime GetAttributeDateTime( this XElement @this, XName name, DateTime defaultValue )
        {
            XAttribute a = @this.Attribute( name );
            return a != null ? XmlConvert.ToDateTime( a.Value, XmlDateTimeSerializationMode.RoundtripKind ) : defaultValue;
        }

        /// <summary>
        /// Gets an <see cref="Int32"/> attribute by name.
        /// </summary>
        /// <param name="this">This <see cref="XElement"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist.</param>
        static public int GetAttributeInt( this XElement @this, XName name, int defaultValue )
        {
            XAttribute a = @this.Attribute( name );
            int i;
            if( a != null && int.TryParse( a.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out i ) ) return i;
            return defaultValue;
        }

        /// <summary>
        /// Gets an enum value.
        /// </summary>
        /// <typeparam name="T">Type of the enum. There is no way (in c#) to constraint the type to Enum - nor to Delegate, this is why 
        /// the constraint restricts only the type to be a value type.</typeparam>
        /// <param name="this">This <see cref="XElement"/>.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="defaultValue">Default value if the attribute does not exist or can not be parsed.</param>
        /// <returns>The parsed value or the default value.</returns>
        static public T GetAttributeEnum<T>( this XElement @this, XName name, T defaultValue ) where T : struct
        {
            T result;
            XAttribute a = @this.Attribute( name );
            if( a == null || !Enum.TryParse( a.Value, out result ) ) result = defaultValue;
            return result;
        }

        #endregion

    }
}
