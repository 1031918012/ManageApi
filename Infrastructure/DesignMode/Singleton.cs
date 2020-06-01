using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class Singleton
    {
        //单线程的单例实现,在多线程的时候，因为在不同线程调用，会产生两个线程同时通过判断的情况,达不到单例的效果
        private static Singleton instance = null;

        private Singleton() { }//默认为空参构造函数,

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }

    public class MultiThreadSingleton
    {
        //多线程的单例实现,使用lock控制,无论多少线程，只会产生一个，直到被销毁才产生下一个
        private static volatile MultiThreadSingleton instance = null;
        //请注意一个关键字volatile，如果去掉这个关键字，还是有可能发生线程不是安全的。volatile保证严格意义的多线程编译器在代码编译时对指令不进行微调。
        private static object lockHelper = new object();
        private MultiThreadSingleton() { }

        public static MultiThreadSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockHelper)
                    {
                        if (instance == null)
                        {
                            instance = new MultiThreadSingleton();
                        }
                    }
                }
                return instance;
            }
        }
    }
    public class StaticSingleton
    {
        //静态单件模式
        private static readonly StaticSingleton instance = new StaticSingleton();

        //public static readonly Static_Singleton instance;
        //static Static_Singleton()
        //{
        //    instance = new Static_Singleton();
        //}
        private StaticSingleton() { }
    }
}
