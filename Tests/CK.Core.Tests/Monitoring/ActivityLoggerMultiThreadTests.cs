#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Core.Tests\Monitoring\ActivityLoggerMultiThreadTests.cs) is part of CiviKey. 
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CK.Core.Tests.Monitoring
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Category( "ActivityMonitor" )]
    public class ActivityMonitorMultiThreadTests
    {
        internal class BuggyActivityMonitorClient : ActivityMonitorClient
        {
            private  IActivityMonitor _monitor;
            internal BuggyActivityMonitorClient( IActivityMonitor monitor )
            {
                _monitor = monitor;
            }

            protected override void OnUnfilteredLog( ActivityMonitorLogData data )
            {
                _monitor.Info().Send( "I'm buggy: I'm logging back in my monitor!" );
                base.OnUnfilteredLog( data );
            }
        }

        internal class NotBuggyActivityMonitorClient : ActivityMonitorClient
        {
            private  int _number;
            internal NotBuggyActivityMonitorClient( int number )
            {
                _number = number;
            }

            protected override void OnUnfilteredLog( ActivityMonitorLogData data )
            {
                Console.WriteLine( "NotBuggyActivityMonitorClient.OnUnfilteredLog n°{0}: {1}", _number, data.Text );
            }
        }

        internal class ActionActivityMonitorClient : ActivityMonitorClient
        {
            Action _log;
            internal ActionActivityMonitorClient( Action log )
            {
                _log = log;
            }

            protected override void OnUnfilteredLog( ActivityMonitorLogData data )
            {
                _log();
            }
        }

        internal class WaitActivityMonitorClient : ActivityMonitorClient
        {
            readonly object _locker = new object();
            bool _done = false;

            readonly object _outLocker = new object();
            bool _outDone = false;

            protected override void OnUnfilteredLog( ActivityMonitorLogData data )
            {
                lock( _locker )
                {
                    lock( _outLocker )
                    {
                        _outDone = true;
                        Monitor.PulseAll( _outLocker );
                    }
                    while( !_done )
                        Monitor.Wait( _locker );
                }
            }

            internal void WaitForOnUnfilteredLog()
            {
                lock( _outLocker )
                    while( !_outDone )
                        Monitor.Wait( _outLocker );
            }

            internal void Free()
            {
                lock( _locker )
                {
                    _done = true;
                    Monitor.PulseAll( _locker );
                }
            }
        }

        [Test]
        public void ExhibeReentrancyAndMultiThreadErrors()
        {
            ActivityMonitor.AutoConfiguration = null;
            ActivityMonitor monitor = new ActivityMonitor();
            monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            WaitActivityMonitorClient client = monitor.Output.RegisterClient( new WaitActivityMonitorClient() );

            Assert.That( monitor.Output.Clients.Count, Is.EqualTo( 2 ) );

            try
            {
                Task.Factory.StartNew( () =>
                {
                    monitor.Info().Send( "Test must work in task" );
                } );

                client.WaitForOnUnfilteredLog();

                Assert.That( () => monitor.Info().Send( "Test must fail" ),
                    Throws.TypeOf( typeof( InvalidOperationException ) ).
                        And.Message.EqualTo( R.ActivityMonitorConcurrentThreadAccess ) );
                
                Assert.That( monitor.Output.Clients.Count, Is.EqualTo( 2 ), "Still 2: Concurrent call: not the fault of the Client." );
            }
            finally
            {
                client.Free();
            }

            Thread.Sleep( 50 );
            monitor.Info().Send( "Test must work after task" );

            monitor.Output.RegisterClient( new ActionActivityMonitorClient( () =>
            {
                Assert.That( () => monitor.Info().Send( "Test must fail reentrant client" ),
                    Throws.TypeOf( typeof( InvalidOperationException ) ).
                        And.Message.EqualTo( R.ActivityMonitorReentrancyError ) );
            } ) );

            monitor.Info().Send( "Test must work after reentrant client" );
            Assert.That( monitor.Output.Clients.Count, Is.EqualTo( 3 ), "The RegisterClient action above is ok: it checks that it triggered a reentrant call." );

            monitor.Output.RegisterClient( new ActionActivityMonitorClient( () =>
            {
                monitor.Info().Send( "Test must fail reentrant client" );
            } ) );

            monitor.Info().Send( "Test must work after reentrant client" );
            Assert.That( monitor.Output.Clients.Count, Is.EqualTo( 3 ), "The BUGGY RegisterClient action above is NOT ok: it let the a reentrant call exception => We have removed it." );

        }

        [Test]
        public void ReentrancyMultiThread()
        {
            IActivityMonitor monitor = new ActivityMonitor();
            monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );

            object lockTasks = new object();
            object lockRunner = new object();
            int enteredThread = 0;

            Action getLock = () =>
            {
                lock( lockTasks )
                {
                    Interlocked.Increment( ref enteredThread );
                    lock( lockRunner )
                        Monitor.Pulse( lockRunner );
                    Monitor.Wait( lockTasks );
                }
            };

            Task[] tasks = new Task[] 
            {            
                new Task( () => { getLock(); monitor.Info().Send( "Test T1" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( new Exception(), "Test T2" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( "Test T3" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( new Exception(), "Test T4" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( "Test T5" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( new Exception(), "Test T6" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( "Test T7" ); } ),
                new Task( () => { getLock(); monitor.Info().Send( new Exception(), "Test T8" ); } )
            };

            Parallel.ForEach( tasks, t => t.Start() );

            lock( lockRunner )
                while( enteredThread < 8 )
                    Monitor.Wait( lockRunner );

            lock( lockTasks )
                Monitor.PulseAll( lockTasks );

            Assert.Throws<AggregateException>( () => Task.WaitAll( tasks ) );

            CollectionAssert.AllItemsAreInstancesOfType( tasks.Where( x => x.IsFaulted ).
                                                                SelectMany( x => x.Exception.Flatten().InnerExceptions ),
                                                                typeof( InvalidOperationException ) );

            Assert.DoesNotThrow( () => monitor.Info().Send( "Test" ) );
        }

        [Test]
        public void ReentrancyMonoThread()
        {
            IActivityMonitor monitor = new ActivityMonitor();
            using( monitor.Output.CreateBridgeTo( TestHelper.ConsoleMonitor.Output.BridgeTarget ) )
            {
                int clientCount = monitor.Output.Clients.Count;
                Assert.That( monitor.Output.Clients.Count, Is.EqualTo( clientCount ) );

                BuggyActivityMonitorClient client = new BuggyActivityMonitorClient( monitor );
                monitor.Output.RegisterClient( client );
                Assert.That( monitor.Output.Clients.Count, Is.EqualTo( clientCount + 1 ) );
                monitor.Info().Send( "Test" );
                Assert.That( monitor.Output.Clients.Count, Is.EqualTo( clientCount ) );

                Assert.DoesNotThrow( () => monitor.Info().Send( "Test" ) );
            }
        }
    }
}
