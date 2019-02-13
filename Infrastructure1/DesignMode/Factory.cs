using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DesignMode
{
    public abstract class CarFactory
    {
        public abstract Car CarCreate();
    }

    public abstract class Car
    {
        public abstract void StartUp();
        public abstract void Run();
        public abstract void Stop();

    }

    public class HongQiCarFactory : CarFactory
    {
        public override Car CarCreate()
        {
            return new HongQiCar();
        }
    }

    public class BMWCarFactory : CarFactory
    {
        public override Car CarCreate()
        {
            return new BMWCar();
        }
    }
    public class HongQiCar : Car
    {
        public override void StartUp()
        {
            Console.WriteLine("Test HongQiCar start-up speed!");
        }
        public override void Run()
        {
            Console.WriteLine("The HongQiCar run is very quickly!");
        }
        public override void Stop()
        {
            Console.WriteLine("The slow stop time is 3 second ");
        }
    }
    public class BMWCar : Car
    {
        public override void StartUp()
        {
            Console.WriteLine("The BMWCar start-up speed is very quickly");
        }
        public override void Run()
        {
            Console.WriteLine("The BMWCar run is quitely fast and safe!!!");
        }
        public override void Stop()
        {
            Console.WriteLine("The slow stop time is 2 second");
        }
    }
}
