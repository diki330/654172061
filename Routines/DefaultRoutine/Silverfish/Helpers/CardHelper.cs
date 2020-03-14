using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HREngine.Bots;

namespace Silverfish.Routines.DefaultRoutine.Silverfish.Helpers
{
    public class CardHelper
    {
        private static readonly Type[] AssemblyTypes;

        static CardHelper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            AssemblyTypes = assembly.GetTypes();
        }

        public static SimTemplate GetCardSimulation(CardDB.cardIDEnum tempCardIdEnum)
        {
            SimTemplate result = new SimTemplate();

            var className = string.Format("Sim_{0}", tempCardIdEnum);
            var list = GetTypeByName(className);
            if (list.Count != 1)
            {
                if (list.Count >= 2)
                {
                    var content = string.Join(",", list.Select(x => x.FullName));
                    throw new Exception(string.Format("Find multiple card simulation class for {0} : {1}",
                        tempCardIdEnum, content));
                }
            }
            else
            {
                var type = list[0];
                var simTemplateInstance = Activator.CreateInstance(type);
                if (simTemplateInstance.GetType().IsSubclassOf(typeof(SimTemplate)))
                {
                    result = simTemplateInstance as SimTemplate;
                }
                else
                {
                    throw new Exception(string.Format("class {0} should inherit from {1}", className,
                        typeof(SimTemplate)));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a all Type instances matching the specified class name with just non-namespace qualified class name.
        /// </summary>
        /// <param name="className">Name of the class sought.</param>
        /// <returns>Types that have the class name specified. They may not be in the same namespace.</returns>
        public static List<Type> GetTypeByName(string className)
        {
            var collection = AssemblyTypes.Where(t => t.Name.Equals(className));
            return collection.ToList();
        }

        public static bool IsCardSimulationImplemented(SimTemplate cardSimulation)
        {
            var type = cardSimulation.GetType();
            var baseType = typeof(SimTemplate);
            bool implemented = type.IsSubclassOf(baseType);
            return implemented;
        }
    }
}
