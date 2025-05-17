using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();


        public void Subscribe<T>(Action<T> handler) where T : ChatEvent
        {
            Type type = typeof(T);
            if(!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            _handlers[type].Add(handler);
        }
        public void Publish(ChatEvent chatEvent)
        {
            Type type = chatEvent.GetType();
            if(_handlers.TryGetValue(type, out var handlers))
            {

                foreach (var handler in handlers)
                {
                    handler.DynamicInvoke(chatEvent);
                }
            }
        }
    }
}
