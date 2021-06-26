using System;
using System.Collections.Generic;

namespace Thismaker.Aba.Client
{
    /// <summary>
    /// Added to a method so that it is marked for subscription during the <see cref="ClientBase{TClient}.SubscribeHub"/>
    /// This attribute is not inherited and must be provided again in an overriding method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited =false, AllowMultiple =false)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// If provided, the method is added to the hub using this name instead.
        /// Otherwise, the normal method name is used.
        /// </summary>
        public string MethodName
        {
            get;set;
        }

        public SubscribeAttribute()
        {
            MethodName = null;
        }

        public SubscribeAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
