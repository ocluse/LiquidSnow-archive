using System;

namespace Thismaker.Aba.Client.SignalR
{
    /// <summary>
    /// Added to a method so that it is marked for subscription during the <see cref="SignalRClientBase{TClient}.SubscribeHub"/>
    /// This attribute is not inherited and must be provided again in an overriding method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// If provided, the method is added to the hub using this name instead.
        /// Otherwise, the normal method name is used.
        /// </summary>
        public string MethodName
        {
            get; set;
        }

        /// <summary>
        /// Creates a new instance of the subscribe attribute
        /// </summary>
        public SubscribeAttribute()
        {
            MethodName = null;
        }

        /// <summary>
        /// Creates a new instance of the subscribe attribute with the specified custom method name
        /// </summary>
        /// <param name="methodName">The name that will be used when subscribing to the hub</param>
        public SubscribeAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
