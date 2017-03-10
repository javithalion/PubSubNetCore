using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Server.MessageHandling
{
    public class Dispatcher
    {
        private static readonly object _locker = new object();
        private static readonly IDictionary<string, ICollection<EndPoint>> _topicSubscribers = new Dictionary<string, ICollection<EndPoint>>();

        public ReadOnlyCollection<EndPoint> GetSubscribers(string topic)
        {
            lock (_locker)
            {
                if (_topicSubscribers.Keys.Contains(topic))
                    return new ReadOnlyCollection<EndPoint>((List<EndPoint>)_topicSubscribers[topic]);
                else
                    return new ReadOnlyCollection<EndPoint>(new List<EndPoint>());
            }
        }

        public void AddSubscriber(string topic, EndPoint subscriber)
        {
            lock (_locker)
            {
                if (!_topicSubscribers.Keys.Contains(topic))
                    _topicSubscribers.Add(topic, new List<EndPoint>());

                _topicSubscribers[topic].Add(subscriber);
            }
        }

        public void RemoveSubscriber(string topic, EndPoint subscriber)
        {
            lock (_locker)
            {
                if (_topicSubscribers.Keys.Contains(topic))
                    _topicSubscribers[topic].Remove(subscriber);
            }
        }

    }
}
