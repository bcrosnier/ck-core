#region LGPL License
/*----------------------------------------------------------------------------
* This file (Mon2Htm\CK.Mon2Htm\SimpleJsonWriter.cs) is part of CiviKey. 
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Mon2Htm
{


    /// <summary>
    /// Awfully simple, step-by-step, JSON writer.
    /// </summary>
    public class SimpleJsonWriter
    {
        StringBuilder _builder;
        bool _hasPrev; // True: a comma is required at the beginning of the current value/array/object.

        public SimpleJsonWriter()
        {
            _builder = new StringBuilder();
        }

        public SimpleJsonWriter OpenObject()
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( '{' );
            _hasPrev = false;
            return this;
        }

        public SimpleJsonWriter CloseObject()
        {
            _builder.Append( '}' );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter OpenArray()
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( '[' );
            _hasPrev = false;
            return this;
        }

        public SimpleJsonWriter CloseArray()
        {
            _builder.Append( ']' );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter WriteUnescapedString( string value )
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( '"' ).Append( value ).Append( '"' );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter WriteEscapedString( string value )
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( "\"" );
            foreach( char c in value )
            {
                switch( c )
                {
                    case '\"':
                        _builder.Append( "\\\"" );
                        break;
                    case '\\':
                        _builder.Append( "\\\\" );
                        break;
                    case '\b':
                        _builder.Append( "\\b" );
                        break;
                    case '\f':
                        _builder.Append( "\\f" );
                        break;
                    case '\n':
                        _builder.Append( "\\n" );
                        break;
                    case '\r':
                        _builder.Append( "\\r" );
                        break;
                    case '\t':
                        _builder.Append( "\\t" );
                        break;
                    default:
                        int i = (int)c;
                        if( i < 32 || i > 127 )
                        {
                            _builder.AppendFormat( "\\u{0:X04}", i );
                        }
                        else
                        {
                            _builder.Append( c );
                        }
                        break;
                }
            }
            _builder.Append( "\"" );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter WritePropertyStart( string propertyName )
        {
            WriteUnescapedString( propertyName );
            _builder.Append( ':' );
            _hasPrev = false;
            return this;
        }

        public SimpleJsonWriter WriteValue( string value )
        {
            return WriteEscapedString( value );
        }

        public SimpleJsonWriter WriteValue( int value )
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( value );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter WriteValue( double value )
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( value.ToString( CultureInfo.InvariantCulture ) );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter WriteValue( float value )
        {
            if( _hasPrev ) _builder.Append( ',' );
            _builder.Append( value.ToString( CultureInfo.InvariantCulture ) );
            _hasPrev = true;
            return this;
        }

        public SimpleJsonWriter WriteValue( DateTime value )
        {
            // Sortable, conforms with ISO8601.
            // Note: Includes neither milliseconds nor timezone information!
            return WriteUnescapedString( value.ToString( "s", CultureInfo.InvariantCulture ) );
        }

        public SimpleJsonWriter WriteProperty( string propertyName, string value )
        {
            return WritePropertyStart( propertyName ).WriteValue( value );
        }

        public SimpleJsonWriter WriteProperty( string propertyName, int value )
        {
            return WritePropertyStart( propertyName ).WriteValue( value );
        }

        public SimpleJsonWriter WriteProperty( string propertyName, float value )
        {
            return WritePropertyStart( propertyName ).WriteValue( value );
        }

        public SimpleJsonWriter WriteProperty( string propertyName, double value )
        {
            return WritePropertyStart( propertyName ).WriteValue( value );
        }

        public SimpleJsonWriter WriteProperty( string propertyName, DateTime value )
        {
            return WritePropertyStart( propertyName ).WriteValue( value );
        }

        public string GetOutput()
        {
            return _builder.ToString();
        }
    }
}
