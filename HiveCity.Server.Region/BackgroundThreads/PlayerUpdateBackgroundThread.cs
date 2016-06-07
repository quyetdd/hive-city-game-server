// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerUpdateBackgroundThread.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   10ms
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.BackgroundThreads
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Region.Models;
    using HiveCity.Server.Region.Models.ServerEvents;

    public class PlayerUpdateBackgroundThread : IBackgroundThread
    {
        public Models.Region Region { get; set; }

        public readonly ILogger Log = LogManager.GetCurrentClassLogger();
        
        private bool isRunning = false;

        public PlayerUpdateBackgroundThread(Region region)
        {
            this.Region = region;
        }

        public void Setup()
        {
        }

        public void Run(object threadContext)
        {
            var timer = new Stopwatch();

            timer.Start();
            this.isRunning = true;

            // Stop the thread consuming if no players in region
            while (this.isRunning)
            {
                try
                {
                    // Every 100 miliseconds
                    if (timer.Elapsed < TimeSpan.FromMilliseconds(100))
                    {
                        // if no players go to sleep for 1 second
                        if (Region.NumPlayers <= 0)
                        {
                            Thread.Sleep(1000);

                            // When a player does join there's no 1 sec delay
                            timer.Restart();
                        }

                        if (100 - timer.ElapsedMilliseconds > 0)
                        {
                            Thread.Sleep(100 - timer.Elapsed.Milliseconds);
                        }

                        continue;
                    }

                    var updateTime = timer.Elapsed;
                    timer.Restart();

                    this.Update(updateTime);
                }
                catch (Exception e)
                {
                    this.Log.ErrorFormat(string.Format("Exception occured in PlayerUpdateBackgroundThread.Run - {0}", e.StackTrace));
                }
            }
        }

        public void SendUpdate(CPlayerInstance instance)
        {
            // check the the player is dirty
            // the broadcast to everyone
            if (instance != null && instance.Physics.Dirty)
            {
                instance.BroadcastMessage(new MoveToLocation(instance));
                instance.Physics.Dirty = false;
            }
        }

        public void Stop()
        {
            // Release system resources
            this.isRunning = false;
        }

        private void Update(TimeSpan elapsed)
        {
            // broadcast all dirty player movements
            // spin up some threads
            Parallel.ForEach(
                this.Region.AllPlayers.Values.Where(w => w.Physics.Dirty && w is CPlayerInstance).Cast<CPlayerInstance>(),
                this.SendUpdate);
        }

    }
}
