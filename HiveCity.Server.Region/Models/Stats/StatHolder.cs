// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatHolder.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatHolder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Stats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using ExitGames.Logging;

    using HiveCity.Server.Region.Calculators;
    using HiveCity.Server.Region.Models.Interfaces;

    using Environment = HiveCity.Server.Region.Calculators.Environment;

    public class StatHolder : IStatHolder
    {
        public ICharacter Character { get; set; }

        private readonly Dictionary<Type, IStat> stats;

        public Dictionary<Type, IStat> Stats
        {
            get { return this.stats; }
        }

        protected Dictionary<IStat, Calculator> Calculators;

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public StatHolder(IEnumerable<IStat> stats)
        {
            this.stats = new Dictionary<Type, IStat>();
            this.Calculators = new Dictionary<IStat, Calculator>();

            foreach (var stat in stats)
            {
                this.Calculators.Add(stat, new Calculator());
                this.stats.Add(stat.GetType(), stat);

                var derived = stat as IDerivedStat;
                if (derived != null)
                {
                    this.Calculators[stat].AddFunction(derived.Functions);
                }
            }
        }

        public float GetStat<T>() where T : class, IStat
        {
            IStat result;
            this.stats.TryGetValue(typeof(T), out result);
            if (result != null)
            {
                return this.CalcStat(result);
            }
            return 0;
        }

        public float GetStat<T>(T stat) where T : class, IStat
        {
            IStat result;
            this.stats.TryGetValue(typeof(T), out result);
            if (result == null)
            {
                this.stats.TryGetValue(((dynamic)stat).GetType(), out result);
            }

            if (result != null)
            {
                return this.CalcStat(result);
            }

            return 0;
        }

        private float CalcStat(IStat stat)
        {
            return this.CalcStat(stat, null);
        }

        private float CalcStat(IStat stat, ICharacter target)
        {
            float returnValue = stat.BaseValue;

            var calculator = this.Calculators[stat];
            var env = new Environment()
            {
                Value = returnValue,
                Character = this.Character,
                Target = target
            };

            calculator.Calculate(env);

            if (env.Value <= 0 && stat.IsNonZero)
            {
                return 1;
            }

            return env.Value;
        }

        public void SetStat<T>(float value) where T : class, IStat
        {
            IStat result;
            this.stats.TryGetValue(typeof(T), out result);
            if (result != null)
            {
                result.BaseValue = value;
            }
        }

        [Serializable]
        private class SerializedStat
        {
            public string StatType { get; set; }

            public float StatValue { get; set; }
        }

        public string SerializeStats()
        {
            var statValues = new List<SerializedStat>();

            foreach (var stat in Stats.Values.ToList())
            {
                statValues.Add(new SerializedStat()
                {
                    StatType = stat.Name,
                    StatValue = stat.BaseValue
                });
            }

            var serializer = new XmlSerializer(typeof(List<SerializedStat>));
            var writer = new StringWriter();
            serializer.Serialize(writer, statValues);

            return writer.ToString();
        }

        public void DeserializeStats(string stats)
        {
            var serializer = new XmlSerializer(typeof(List<SerializedStat>));
            var reader = new StringReader(stats);

            foreach (var stat in (List<SerializedStat>)serializer.Deserialize(reader))
            {
                var result = this.stats.Values.FirstOrDefault(s => s.Name == stat.StatType);
                if (result != null)
                {
                    result.BaseValue = stat.StatValue;
                }
            }
        }
    }
}
