#region LGPL License
/*----------------------------------------------------------------------------
* This file (ActivityMonitorAdapters\CK.Core.ActivityMonitor.NLogAdapter\NLogAdapter.cs) is part of CiviKey. 
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

using CK.Core.ActivityMonitorAdapters.NLogImpl;

namespace CK.Core.ActivityMonitorAdapters
{
    /// <summary>
    /// Startup class: <see cref="Initialize"/> makes all new <see cref="ActivityMonitor"/> routes their output to NLog loggers
    /// named with the monitor's topic.
    /// </summary>
    public static class NLogAdapter
    {
        static bool _isInitialized;
        static object _lock = new object();

        /// <summary>
        /// Causes all newly created ActivityMonitors to automatically output to NLog loggers (based on ActivityMonitors' topics).
        /// </summary>
        public static void Initialize()
        {
            lock( _lock )
            {
                if( !_isInitialized )
                {
                    ActivityMonitor.AutoConfiguration += ( monitor ) =>
                    {
                        NLogTopicBasedClient client = new NLogTopicBasedClient( monitor.Topic );
                        monitor.Output.RegisterClient( client );
                    };
                    _isInitialized = true;
                }
            }
        }
    }
}
