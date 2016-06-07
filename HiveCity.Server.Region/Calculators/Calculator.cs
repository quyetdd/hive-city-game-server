using System.Collections.Generic;
using HiveCity.Server.Region.Models;
using HiveCity.Server.Region.Models.Interfaces;

namespace HiveCity.Server.Region.Calculators
{
    public class Calculator
    {
        private readonly List<IFunction> functionList = new List<IFunction>();

        public void AddFunction(IFunction function)
        {
            this.functionList.Add(function);

            // compare two ints and sort
            this.functionList.Sort((x,y) => x.Order.CompareTo(y.Order));
        }

        public void AddFunction(IEnumerable<IFunction> functions)
        {
            foreach (var function in functions)
            {
                this.functionList.Add(function);
            }

            // compare two ints and sort
            this.functionList.Sort((x, y) => x.Order.CompareTo(y.Order));
        }
        
        public void RemoveFunction(IEnumerable<IFunction> functions)
        {
            foreach (var function in functions)
            {
                this.functionList.Remove(function);  
            }
        }

        public List<IStat> RemoveOwner(CObject owner)
        {
            var modifiedStats = new List<IStat>();

            foreach (var function in this.functionList)
            {
                if (function.Owner == owner)
                {
                    modifiedStats.Add(function.Stat);
                    this.RemoveFunction(function);
                }
            }

            return modifiedStats;
        }

        public void Calculate(Environment env)
        {
            foreach (var function in this.functionList)
            {
                function.Calc(env);
            }
        }

        private void RemoveFunction(IFunction function)
        {
            this.functionList.Remove(function);
        }
    }
}
