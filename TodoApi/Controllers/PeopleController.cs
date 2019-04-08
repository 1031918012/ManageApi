using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ManageApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IPeopleService _people;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="people"></param>
        public PeopleController(IPeopleService people)
        {
            _people = people;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddOnePeople")]
        public OkResult AddOnePeople(string name)
        {
            throw new Exception("yichangceshi");
           // return Ok();
        }
        /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>
        [HttpPost("AddOPeople")]
        public Tuple<TimeSpan, TimeSpan, TimeSpan> AddOPeople()
        {
            List<People> list = new List<People>();
            People people = new People
            {
                BankCard = "123",
                Creator = "1",
                IDCard = "420114199602191215",
                Name = "1",
                PeopleID = Guid.NewGuid()
            };
            for (int i = 0; i < 10000; i++)
            {
                list.Add(people);
            }
            DateTime now = DateTime.Now;
            list.ForEach(s => 
            {
                _people.AddPeople(s);
            });
            DateTime end = DateTime.Now;
            _people.Addpeoplelist(list);
            DateTime date = DateTime.Now;
            _people.Addpeoplelistasyn(list);
            DateTime time = DateTime.Now;
            TimeSpan dateTime = end - now;
            TimeSpan dateTime2 = date - end;
            TimeSpan dateTime3 = time - date;
            Tuple<TimeSpan, TimeSpan, TimeSpan> a = new Tuple<TimeSpan, TimeSpan, TimeSpan>(dateTime, dateTime2,dateTime3);
            return a;
        }
    }
}