﻿#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Core\SimpleMultiAction.cs) is part of CiviKey. 
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
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Core
{
    /// <summary>
    /// Encapsulates multiple actions. This is thread-safe and can be exposed as static property.
    /// Changes (via <see cref="Append"/> or <see cref="Set"/>) will be effective on future calls to <see cref="Apply"/>.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public sealed class SimpleMultiAction<T>
    {
        Action<T>[] _actions;

        /// <summary>
        /// Appends one or more actions.
        /// These new actions will be executed only to subsequent calls to <see cref="Apply"/> (not to any previously seen objects).
        /// </summary>
        /// <param name="actions">One or more actions (all of them must be not null).</param>
        public void Append( params Action<T>[] actions )
        {
            if( actions.Length == 0 ) return;
            if( actions.Any( c => c == null ) ) throw new ArgumentException( R.SimpleMultiActionNullAction );
            Util.InterlockedSet( ref _actions, existing =>
                {
                    if( existing == null || existing.Length == 0 ) return actions;
                    Action<T>[] newOne = new Action<T>[existing.Length + actions.Length];
                    Array.Copy( existing, newOne, existing.Length );
                    Array.Copy( actions, 0, newOne, existing.Length, actions.Length );
                    return newOne;
                } ); 
        }

        /// <summary>
        /// Resets the actions.
        /// These new actions will be executed only to subsequent calls to <see cref="Apply"/> (not to any previously seen objects).
        /// </summary>
        /// <param name="actions">Zero, one or more actions.</param>
        public void Set( params Action<T>[] actions )
        {
            if( actions.Any( c => c == null ) ) throw new ArgumentException( R.SimpleMultiActionNullAction );
            Interlocked.Exchange( ref _actions, actions );
        }

        /// <summary>
        /// Clears the current actions.
        /// </summary>
        public void Clear()
        {
            Interlocked.Exchange( ref _actions, Util.EmptyArray<Action<T>>.Empty );
        }

        /// <summary>
        /// Applies current actions to the given object.
        /// </summary>
        /// <param name="obj">The object onto which current actions must be applied. Must not be null (when <typeparamref name="T"/> is a reference type).</param>
        public void Apply( T obj )
        {
            if( obj == null ) throw new ArgumentNullException( "obj" );
            var cfg = _actions;
            if( cfg != null )
            {
                foreach( var c in cfg ) c( obj );
            }
        }
    }

}
