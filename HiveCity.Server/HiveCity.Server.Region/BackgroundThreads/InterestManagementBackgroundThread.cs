// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestManagementBackgroundThread.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Characters, pets, anything that can move, mobs everything.
//   Smaller regions mayvbe? for fps intensive.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.BackgroundThreads
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Region.Models.Interfaces;

    public class InterestManagementBackgroundThread : IBackgroundThread
    {
        private const int FullUpdateTimer = 100;

        private int fullUpdateTimer = FullUpdateTimer;

        private bool updatePass = true;

        private bool isRunning = false;

        public InterestManagementBackgroundThread(Models.Region region)
        {
            this.Region = region;
        }

        private Models.Region Region { get; set; }

        public void Setup()
        {
        }

        public void Run(object threadContext)
        {
            var timer = new Stopwatch();
            timer.Start();

            this.isRunning = true;

            while (this.isRunning)
            {
                // Every 1.25 seconds
                if (timer.Elapsed < TimeSpan.FromMilliseconds(1250))
                {
                    if (this.Region.NumPlayers <= 0)
                    {
                        Thread.Sleep(1000);
                        timer.Restart();
                    }

                    Thread.Sleep(1250 - timer.Elapsed.Milliseconds);
                    continue;
                }

                var updateTime = timer.Elapsed;
                timer.Restart();

                // Every 2 mins update
                this.Update(updateTime, FullUpdateTimer == this.fullUpdateTimer, this.updatePass);
                if (this.fullUpdateTimer > 0)
                {
                    this.fullUpdateTimer--;
                }
                else
                {
                    this.fullUpdateTimer = FullUpdateTimer;
                }

                // Every 2.5 seconds
                this.updatePass = !this.updatePass;
            }
        }

        private void Update(TimeSpan elapsed, bool fullUpdate, bool forgetObjects)
        {
            this.Region.ForEachObject(obj =>
            {
                if (obj.IsVisible)
                {
                    // TODO:  (GuardAttackAgroMob && obj is CGuardInstance) // Guards which don't exist yet
                    const bool Aggressive = false;

                    obj.KnownList.ForgetObjects(forgetObjects);

                    // Fullupdate = everyone will get handled
                    // Player if not full update then just the players
                    if (obj is IPlayable || Aggressive || fullUpdate)
                    {
                        foreach (var visible in Region.VisibleObjects)
                        {
                            if (visible != obj)
                            {
                                obj.KnownList.AddKnownObject(visible);
                            }
                        }
                    }

                    // NPC's aggressive mobs, pets.
                    else if (obj is ICharacter)
                    {
                        // 24mins
                        foreach (var playable in Region.GetVisiblePlayable(obj))
                        {
                            var visible = (IObject)playable;
                            if (visible != obj)
                            {
                                obj.KnownList.AddKnownObject(visible);
                            }
                        }
                    }
                }
            });
        }

        public void Stop()
        {
            this.isRunning = false;
        }
    }
}
