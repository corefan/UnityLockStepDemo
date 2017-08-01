﻿using LockStepDemo.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockStepDemo.Event
{
    public delegate void MessageHandle<T>(SyncSession session, T e);
    class EventService
    {
        private static Dictionary<Type, EventDispatcher> mTypeEventDic = new Dictionary<Type, EventDispatcher>();
        private static Dictionary<Type, EventDispatcher> mTypeUseOnceEventDic = new Dictionary<Type, EventDispatcher>();

        /// <summary>
        /// 添加事件及回调
        /// </summary>
        /// <param name="type">事件枚举</param>
        /// <param name="handle">回调</param>
        /// <param name="isUseOnce"></param>
        public static void AddTypeEvent<T>(MessageHandle<T> handle, bool isUseOnce = false)
        {
            GetEventDispatcher<T>(isUseOnce).m_CallBack += handle;
        }

        /// <summary>
        /// 移除某类事件的一个回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handle"></param>
        public static void RemoveTypeEvent<T>(MessageHandle<T> handle, bool isUseOnce = false)
        {
            GetEventDispatcher<T>(isUseOnce).m_CallBack -= handle;
        }

        /// <summary>
        /// 移除某类事件
        /// </summary>
        /// <param name="type"></param>
        public static void RemoveTypeEvent<T>(bool isUseOnce = false)
        {
            GetEventDispatcher<T>(isUseOnce).m_CallBack = null;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public static void DispatchTypeEvent<T>(SyncSession session,T e)
        {
            GetEventDispatcher<T>(false).Call(session,e);

            //只调用一次的调用后就清除
            GetEventDispatcher<T>(true).Call(session,e);
            GetEventDispatcher<T>(true).m_CallBack = null;
        }

        static EventDispatcher<T> GetEventDispatcher<T>(bool isOnce)
        {
            Type type = typeof(T);

            if (isOnce)
            {
                if (mTypeUseOnceEventDic.ContainsKey(type))
                {
                    return (EventDispatcher<T>)mTypeUseOnceEventDic[type];
                }
                else
                {
                    EventDispatcher<T> temp = new EventDispatcher<T>();
                    mTypeUseOnceEventDic.Add(type, temp);
                    return temp;
                }
            }
            else
            {
                if (mTypeEventDic.ContainsKey(type))
                {
                    return (EventDispatcher<T>)mTypeEventDic[type];
                }
                else
                {
                    EventDispatcher<T> temp = new EventDispatcher<T>();
                    mTypeEventDic.Add(type, temp);
                    return temp;
                }
            }
        }

        abstract class EventDispatcher { }

        class EventDispatcher<T> : EventDispatcher
        {
            public MessageHandle<T> m_CallBack;

            public void Call(SyncSession session ,T e)
            {
                if (m_CallBack != null)
                {
                    try
                    {
                        m_CallBack(session,e);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.ToString());
                        //Debug.LogError(exc.ToString());
                    }
                }
            }
        }
    }
}