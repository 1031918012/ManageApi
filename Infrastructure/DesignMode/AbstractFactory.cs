using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class Road { }
    public class Tunnel { }
    public class Jungle { }
    public class Building { }
    public class RoadFactory
    {
        public static Road CreateRoad()
        {
            return new Road();
        }
    }
    //Road road = roadFactory.CreateRoad();调用
    class RoadFactory2//简单工厂，问题：不能应对"不同系列对象"的变化。比如有不同风格的场景---对应不同风格的道路，房屋、地道....
    {
        public static Road CreateRoad()
        {
            return new Road();
        }
        public static Building CreateBuilding()
        {
            return new Building();
        }
        public static Tunnel CreateTunnel()
        {
            return new Tunnel();
        }
        public static Jungle CreateJungle()
        {
            return new Jungle();
        }
        //Road road = RoadFactory.CreateRoad();调用
        // Building building = RoadFactory.CreateBuilding();
        // Tunnel tunnel = RoadFactory.CreateTunnel();
        // Jungle jungle = RoadFactory.CreateJungle();
    }

    abstract class AbstractFactory
    {
        public abstract AbstractProductA CreateProductA();
        public abstract AbstractProductB CreateProductB();
    }

    abstract class AbstractProductA
    {
        public abstract void Interact(AbstractProductB b);
    }

    abstract class AbstractProductB
    {
        public abstract void Interact(AbstractProductA a);
    }


    class Client
    {
        private AbstractProductA AbstractProductA;
        private AbstractProductB AbstractProductB;
        public Client(AbstractFactory factory)
        {
            AbstractProductA = factory.CreateProductA();
            AbstractProductB = factory.CreateProductB();
        }
        public void Run()
        {
            AbstractProductB.Interact(AbstractProductA);
            AbstractProductA.Interact(AbstractProductB);
        }
    }
    class ConcreteFactory1 : AbstractFactory
    {
        public override AbstractProductA CreateProductA()
        {
            return new ProductA1();
        }
        public override AbstractProductB CreateProductB()
        {
            return new ProductB1();
        }
    }
    class ConcreteFactory2 : AbstractFactory
    {
        public override AbstractProductA CreateProductA()
        {
            return new ProdcutA2();
        }
        public override AbstractProductB CreateProductB()
        {
            return new ProductB2();
        }
    }
    class ProductA1 : AbstractProductA
    {
        public override void Interact(AbstractProductB b)
        {
            Console.WriteLine(this.GetType().Name + "interact with" + b.GetType().Name);
        }
    }
    class ProductB1 : AbstractProductB
    {
        public override void Interact(AbstractProductA a)
        {
            Console.WriteLine(this.GetType().Name + "interact with" + a.GetType().Name);
        }
    }
    class ProdcutA2 : AbstractProductA
    {
        public override void Interact(AbstractProductB b)
        {
            Console.WriteLine(this.GetType().Name + "interact with" + b.GetType().Name);
        }
    }
    class ProductB2 : AbstractProductB
    {
        public override void Interact(AbstractProductA a)
        {
            Console.WriteLine(this.GetType().Name + "interact with" + a.GetType().Name);
        }
    }
    //public static void Main()//调用
    //{
    //    // Abstractfactory1
    //    AbstractFactory factory1 = new ConcreteFactory1();
    //    Client c1 = new Client(factory1);
    //    c1.Run();
    //    // Abstractfactory2
    //    AbstractFactory factory2 = new ConcreteFactory2();
    //    Client c2 = new Client(factory2);
    //    c2.Run();
    //}
}
