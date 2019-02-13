using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DesignMode
{
    public abstract class NormalActor
    {
        public abstract NormalActor clone();
    }
    public class NormalActorA : NormalActor
    {
        public override NormalActor clone()
        {
            Console.WriteLine("NormalActorA is call");
            return (NormalActor)MemberwiseClone();
        }
    }
    public class NormalActorB : NormalActor
    {
        public override NormalActor clone()
        {
            Console.WriteLine("NormalActorB  was called");
            return (NormalActor)MemberwiseClone();

        }
    }
    public class GameSystem
    {
        public void Run(NormalActor normalActor)
        {
            NormalActor normalActor1 = normalActor.clone();
            NormalActor normalActor2 = normalActor.clone();
            NormalActor normalActor3 = normalActor.clone();
            NormalActor normalActor4 = normalActor.clone();
            NormalActor normalActor5 = normalActor.clone();
        }
    }
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        GameSystem gameSystem = new GameSystem();

    //        gameSystem.Run(new NormalActorA());//克隆的种类为a，调用run中
    //    }
    //}
    public abstract class FlyActor
    {
        public abstract FlyActor clone();
    }

    public class FlyActorB : FlyActor
    {
        /// <summary>
        /// 浅拷贝，如果用深拷贝，可使用序列化
        /// </summary>
        /// <returns></returns>
        public override FlyActor clone()
        {
            return (FlyActor)MemberwiseClone();
        }
    }
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        GameSystem gameSystem = new GameSystem();

    //        gameSystem.Run(new NormalActorA(), new FlyActorB());//这里的run可以编写为一个通用方法
    //    }
    //}
}
