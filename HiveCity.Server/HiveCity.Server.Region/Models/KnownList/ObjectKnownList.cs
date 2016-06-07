// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectKnownList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObjectKnownList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.KnownList
{
    using System.Collections.Concurrent;
    using System.Linq;

    using HiveCity.Server.Region.Models.Interfaces;

    public class ObjectKnownList : IKnownList
    {
        public ObjectKnownList()
        {
            this.KnownObjects = new ConcurrentDictionary<int, IObject>();
        }

        public IObject Owner { get; set; }

        // Allows add/remove for threaded, locks, etc
        protected ConcurrentDictionary<int, IObject> KnownObjects;

        public virtual bool AddKnownObject(IObject obj)
        {
            if (obj == null)
            {
                return false;
            }

            // if owner.Instance == -1 - Assume it is a GM - see everything
            if (this.Owner.InstanceId != -1 && obj.InstanceId != this.Owner.InstanceId)
            {
                return false;
            }

            // No Duplicates
            if (this.KnowsObject(obj))
            {
                return false;
            }

            // in radius then add it so not everying in region
            // 3d for fliers
            if (!Util.IsInShortRange(this.DistanceToWatchObject(obj), this.Owner, obj, true))
            {
                return false;
            }

            this.KnownObjects.TryAdd(obj.ObjectId, obj);
            return true;
        }

        public bool KnowsObject(IObject obj)
        {
            // Super quick lookup
            return this.Owner == obj || this.KnownObjects.ContainsKey(obj.ObjectId);
        }

        public virtual void RemoveAllKnownObjects()
        {
            this.KnownObjects.Clear();
        }

        public virtual bool RemoveKnownObject(IObject obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.KnowsObject(obj))
            {
                this.KnownObjects.TryRemove(obj.ObjectId, out obj);
                return true;
            }

            return false;
        }

        public virtual void FindObjects() { }

        /// <summary>
        /// Forget multiple objects
        /// Letting Garbage collection handle disposing.
        /// </summary>
        /// <param name="fullCheck"></param>
        public virtual void ForgetObjects(bool fullCheck)
        {
            // stop enumerations error / no locking
            var values = this.KnownObjects.Values.ToList();

            var iter = values.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    values.Remove(iter.Current);
                    continue;
                } 

                // Pvp
                // Full check to check whether players are still in view range
                if(!fullCheck && !(iter.Current is IPlayable))
                {
                    continue;
                }

                if (!iter.Current.IsVisible || !Util.IsInShortRange(this.DistanceToForgetObject(iter.Current), Owner, iter.Current, true))
                {
                    values.Remove(iter.Current);
                }
            }

            var newKnownObjects = new ConcurrentDictionary<int, IObject>();
            foreach (var value in values)
            {
                 newKnownObjects.TryAdd(value.ObjectId, value);   
            }

            this.KnownObjects = newKnownObjects;
        }

        public virtual int DistanceToForgetObject(IObject obj)
        {
            return 0;
        }

        public virtual int DistanceToWatchObject(IObject obj)
        {
            return 0;
        }
    }
}
