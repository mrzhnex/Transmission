using Core.Handlers;
using System;
using System.Collections.Generic;

namespace Core.Main
{
    public class EventManager
    {
		private Dictionary<Type, List<IEventHandler>> Events { get; set; } = new Dictionary<Type, List<IEventHandler>>();

		internal EventManager() { }

		public void ExecuteEvent<T>(Events.Event ev) where T : IEventHandler
        {
			if (!Events.ContainsKey(typeof(T)))
				return;
            foreach (IEventHandler eventHandler in Events[typeof(T)])
            {
                ev.ExecuteHandler(eventHandler);
            }
        }
		public void AddEventHandlers(IEventHandler handler)
		{
			foreach (Type type in handler.GetType().GetInterfaces())
			{
				if (typeof(IEventHandler).IsAssignableFrom(type))
				{
					AddEventHandler(type, handler);
				}
			}
		}
		private void AddEventHandler(Type type, IEventHandler handler)
		{
			if (Events.ContainsKey(type))
            {

				Events[type].Add(handler);
			}
            else
			{
				Events.Add(type, new List<IEventHandler> { handler });
			}
		}
	}
}