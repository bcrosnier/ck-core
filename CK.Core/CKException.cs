#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Core\CKException.cs) is part of CiviKey. 
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
using System.Reflection;
using System.Runtime.Serialization;

namespace CK.Core
{
    /// <summary>
    /// Basic <see cref="Exception"/> that eases message formatting thanks to its contructors
    /// and provides an Exception wrapper around <see cref="CKExceptionData"/>.
    /// </summary>
    [Serializable]
    public class CKException : Exception
    {
        [NonSerialized]
        CKExceptionData _exceptionData;

        /// <summary>
        /// Initializes a new <see cref="CKException"/>.
        /// </summary>
        /// <param name="message">Simple message.</param>
        public CKException( string message )
            : base( message )
        {
            SerializeObjectState += DoSerialize;
        }

        /// <summary>
        /// Initializes a new <see cref="CKException"/>.
        /// </summary>
        /// <param name="message">Simple message.</param>
        /// <param name="innerException">Exception that caused this one.</param>
        public CKException( string message, Exception innerException )
            : base( message, innerException )
        {
            SerializeObjectState += DoSerialize;
        }

        /// <summary>
        /// Initializes a new <see cref="CKException"/>.
        /// </summary>
        /// <param name="messageFormat">Format string with optional placeholders.</param>
        /// <param name="args">Varying number of arguments to format.</param>
        public CKException( string messageFormat, params object[] args )
            : this( String.Format( messageFormat, args ) )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="CKException"/> with an <see cref="Exception.InnerException"/>.
        /// </summary>
        /// <param name="innerException">Exception that caused this one.</param>
        /// <param name="messageFormat">Format string with optional placeholders.</param>
        /// <param name="args">Varying number of arguments to format.</param>
        public CKException( Exception innerException, string messageFormat, params object[] args )
            : this( String.Format( messageFormat, args ), innerException )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="CKException"/> with an <see cref="ExceptionData"/>.
        /// The message of this exception is the <see cref="CKExceptionData.Message"/>.
        /// Use the static <see cref="CreateFrom"/> to handle null data (a null CKException will be returned).
        /// </summary>
        /// <param name="data">The exception data. Must not be null.</param>
        public CKException( CKExceptionData data )
            : this( data.Message )
        {
            _exceptionData = data;
        }

        /// <summary>
        /// Creates a <see cref="CKException"/> from a <see cref="CKExceptionData"/>. This method returns null when data is null.
        /// This is the symmetric of <see cref="CKExceptionData.CreateFrom"/>.
        /// </summary>
        /// <param name="data">Data of an exception for which a <see cref="CKException"/> wrapper must be created. Can be null: null is returned.</param>
        /// <returns>The exception that wraps the data.</returns>
        static public CKException CreateFrom( CKExceptionData data )
        {
            if( data == null ) return null;
            return new CKException( data );
        }
        
        /// <summary>
        /// Gets the <see cref="CKExceptionData"/> if it exists: use <see cref="EnsureExceptionData"/> to 
        /// create if this is null, a data that describes this exception.
        /// </summary>
        public CKExceptionData ExceptionData { get { return _exceptionData; } }

        /// <summary>
        /// If <see cref="ExceptionData"/> is null, this method creates the <see cref="CKExceptionData"/> with the details
        /// from this exception.
        /// </summary>
        /// <returns>The <see cref="CKExceptionData"/> that describes this exception.</returns>
        public CKExceptionData EnsureExceptionData()
        {
            if( _exceptionData == null )
            {
                _exceptionData = new CKExceptionData( Message, "CKException", GetType().AssemblyQualifiedName, StackTrace, CKExceptionData.CreateFrom( InnerException ), null, null, null, null );
            }
            return _exceptionData; 
        }

        void DoSerialize( object sender, SafeSerializationEventArgs e )
        {
            if( _exceptionData != null ) e.AddSerializedState( new SerialData() { ExData = _exceptionData } );
        }

        /// <summary>
        /// Implements the ISafeSerializationData interface: this is the recommended way starting with .Net 4 
        /// to be able to use this in partially trusted environment (the GetObjectData method is now marked with the SecurityCriticalAttribute).
        /// </summary>
        [Serializable]
        struct SerialData : ISafeSerializationData
        {
            /// <summary>
            /// The exception data from <see cref="CKException"/> that must be serialized.
            /// </summary>
            public CKExceptionData ExData;

            void ISafeSerializationData.CompleteDeserialization( object obj )
            {
                ((CKException)obj)._exceptionData = ExData;
            }
        }


    }
}
