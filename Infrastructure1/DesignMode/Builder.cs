using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Builder
{
    public abstract class Builder
    {
        public abstract void BuildDoor();
        public abstract void BuildWall();
        public abstract void BuildWindows();
        public abstract void BuildFloor();
        public abstract void BuildHouseCeiling();

        public abstract House GetHouse();

        public class House
        {
        }
    }
    public class Director
    {
        public void Construct(Builder builder)
        {
            builder.BuildWall();
            builder.BuildHouseCeiling();
            builder.BuildDoor();
            builder.BuildWindows();
            builder.BuildFloor();
        }
    }
    public class ChineseBuilder : Builder
    {
        private House ChineseHouse = new House();
        public override void BuildDoor()
        {
            Console.WriteLine("this Door 's style of Chinese");
        }
        public override void BuildWall()
        {
            Console.WriteLine("this Wall 's style of Chinese");
        }
        public override void BuildWindows()
        {
            Console.WriteLine("this Windows 's style of Chinese");
        }
        public override void BuildFloor()
        {
            Console.WriteLine("this Floor 's style of Chinese");
        }
        public override void BuildHouseCeiling()
        {
            Console.WriteLine("this Ceiling 's style of Chinese");
        }
        public override House GetHouse()
        {
            return ChineseHouse;
        }

    }
    class RomanBuilder : Builder
    {
        private House RomanHouse = new House();
        public override void BuildDoor()
        {
            Console.WriteLine("this Door 's style of Roman");
        }
        public override void BuildWall()
        {
            Console.WriteLine("this Wall 's style of Roman");
        }
        public override void BuildWindows()
        {
            Console.WriteLine("this Windows 's style of Roman");
        }
        public override void BuildFloor()
        {
            Console.WriteLine("this Floor 's style of Roman");
        }
        public override void BuildHouseCeiling()
        {
            Console.WriteLine("this Ceiling 's style of Roman");
        }
        public override House GetHouse()
        {
            return RomanHouse;
        }
    //public class Client
    //{
    //    public static void Main(string[] args)
    //    {
    //        Director director = new Director();

    //        Builder instance;

    //        Console.WriteLine("Please Enter House No:");

    //        string No = Console.ReadLine();

    //        string houseType = ConfigurationSettings.AppSettings["No" + No];

    //        instance = (Builder)Assembly.Load("House").CreateInstance("House." + houseType);

    //        director.Construct(instance);

    //        House house = instance.GetHouse();
    //        house.Show();

    //        Console.ReadLine();
    //    }
    }
}
