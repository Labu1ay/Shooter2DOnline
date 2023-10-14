using System;
using System.Collections.Generic;
using System.Linq;

public static class Messenger {
    public delegate void BroadcastBundleHandler(Bundle bundle);

    public delegate void BroadcastHandler();

    private static SortedList<MessengerKey, BroadcastHandler> BROADCAST_HANDLERS =
        new SortedList<MessengerKey, BroadcastHandler>(new MessengerComparator());

    private static SortedList<MessengerKey, BroadcastBundleHandler> BROADCAST_BUNDLE_HANDLERS =
        new SortedList<MessengerKey, BroadcastBundleHandler>(new MessengerComparator());

    private static readonly Bundle EMPTY_BUNDLE = new Bundle();

    public static void AddListener(MessengerKey eventType, BroadcastHandler callback) {
        if (!BROADCAST_HANDLERS.ContainsKey(eventType)) {
            BROADCAST_HANDLERS.Add(eventType, null);
        }

        BROADCAST_HANDLERS[eventType] += callback;
    }

    public static void AddListener(MessengerKey eventType, BroadcastBundleHandler callback) {
        if (!BROADCAST_BUNDLE_HANDLERS.ContainsKey(eventType)) {
            BROADCAST_BUNDLE_HANDLERS.Add(eventType, null);
        }

        BROADCAST_BUNDLE_HANDLERS[eventType] += callback;
    }

    public static void RemoveListener(MessengerKey eventType, BroadcastHandler callback) {
        if (BROADCAST_HANDLERS.ContainsKey(eventType)) {
            BROADCAST_HANDLERS[eventType] -= callback;
            if (BROADCAST_HANDLERS[eventType] == null) {
                BROADCAST_HANDLERS.Remove(eventType);
            }
        }
    }

    public static void RemoveListener(MessengerKey eventType, BroadcastBundleHandler callback) {
        if (callback != null) BROADCAST_BUNDLE_HANDLERS[eventType] -= callback;
        if (BROADCAST_BUNDLE_HANDLERS[eventType] == null) {
            BROADCAST_BUNDLE_HANDLERS.Remove(eventType);
        }
    }

    public static void Broadcast(MessengerKey eventType, Bundle? bundle = null) {
        if (Array.BinarySearch(BROADCAST_HANDLERS.Keys.ToArray(), eventType) >= 0)
            BROADCAST_HANDLERS[eventType]();
        if (Array.BinarySearch(BROADCAST_BUNDLE_HANDLERS.Keys.ToArray(), eventType) >= 0)
            BROADCAST_BUNDLE_HANDLERS[eventType](bundle ?? EMPTY_BUNDLE);
    }
}

class MessengerComparator : IComparer<MessengerKey> {
    public int Compare(MessengerKey x, MessengerKey y) {
        if (x.GetHashCode() == y.GetHashCode()) return 0;
        if (x.GetHashCode() > y.GetHashCode()) return 1;
        else return -1;
    }
}

public struct Bundle {
    private Dictionary<string, object> _data;

    public T Get<T>(string key) {
        InitData();
        return (T)(_data.ContainsKey(key) ? _data[key] : default(T));
    }

    private void InitData() {
        if (_data == null)
            _data = new Dictionary<string, object>();
    }

    public Bundle Set(string key, object value) {
        InitData();
        _data[key] = value;
        return this;
    }

    public void Clear() {
        _data?.Clear();
    }
}