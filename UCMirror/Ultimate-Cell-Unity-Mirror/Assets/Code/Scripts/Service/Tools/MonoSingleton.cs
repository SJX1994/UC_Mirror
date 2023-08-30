using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    ///<summary>
    ///脚本单例类,负责为唯一脚本创建实例
    ///<summary>

    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> //注意此约束为T必须为其本身或子类
    {
        /*
        相较于直接在需要唯一创建的脚本中创建实例，Awake初始化的过程需要解决的问题
        1.代码重复
        2.在Awake里面初始化，其它脚本在Awake中调用其可能会为Null的异常情况
         */

        //解决1：使用泛型创建实例   解决2：使用按需加载（即有其它脚本调用时在get中加载）

        private static T instance; //创建私有对象记录取值，可只赋值一次避免多次赋值

        public static T Instance
        {
            //实现按需加载
            get
            {
                //当已经赋值，则直接返回即可
                if (instance != null) return instance;

                instance = FindObjectOfType<T>();

                //为了防止脚本还未挂到物体上，找不到的异常情况，可以自行创建空物体挂上去
                if (instance == null)
                {
                    //如果创建对象，则会在创建时调用其身上脚本的Awake即调用T的Awake(T的Awake实际上是继承的父类的）
                    //所以此时无需为instance赋值，其会在Awake中赋值，自然也会初始化所以无需init()
                    /*instance = */
                    new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
                }
                else instance.Init(); //保证Init只执行一次

                return instance;

            }
        }

        private void Awake()
        {
            //若无其它脚本在Awake中调用此实例，则可在Awake中自行初始化instance
            instance = this as T;
            //初始化
            Init();
        }

        //子类对成员进行初始化如果放在Awake里仍会出现Null问题所以自行制作一个init函数解决（可用可不用）
        protected virtual void Init()
        {

        }
    }

}

