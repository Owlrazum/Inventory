using System.Collections.Generic;
using UnityEngine;

namespace SNG.DataStructures
{
    public class ListID<TUnityObject> where TUnityObject : Object
    {
        private List<TUnityObject> _list;

        public ListID(int initialCapasity = 4)
        {
            _list = new List<TUnityObject>(initialCapasity);
        }

        public bool Contains(TUnityObject element)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].GetInstanceID() == element.GetInstanceID())
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(TUnityObject element)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].GetInstanceID() == element.GetInstanceID())
                {
                    Debug.Log("Already present " + element.GetInstanceID());
                    return;
                }
            }

            _list.Add(element);
#if UNITY_EDITOR
            if (_list.Count > 20)
            {
                Debug.LogError("This class was not intented to use with big number of elements");
            }
#endif
        }

        public void Remove(TUnityObject element)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].GetInstanceID() == element.GetInstanceID())
                {
                    _list.RemoveAt(i);
                    return;
                }
            }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public TUnityObject this[int i]
        {
            get { return _list[i]; }
            set { _list[i] = value; }
        }

        public TUnityObject Last
        {
            get { return _list[_list.Count - 1]; }
        }

        public int Count { get { return _list.Count; } }
    }
}