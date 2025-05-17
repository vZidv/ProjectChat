using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public interface IEventAggregator
    {
        void Subscribe<T>(Action<T> handler) where T : ChatEvent;
        void Publish(ChatEvent @event);
    }
}
