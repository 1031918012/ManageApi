using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public TimeSpan AddOPeople()
        {
            List<People> list = new List<People>();
            for (int i = 0; i < 10000; i++)
            {
                People people = new People
                {
                    PeopleID = Guid.NewGuid(),
                    BankCard = "123",
                    Creator = "1",
                    IDCard = "420114199602191215",
                    Name = "1"
                };
                list.Add(people);
            }
            DateTime now = DateTime.Now;
            list.ForEach(s =>
            {
                _people.AddPeople(s);
            });
            return DateTime.Now - now;
        } /// <summary>
          /// 
          /// </summary>
          /// <returns></returns>
        [HttpPost("AddOPeople1")]
        public TimeSpan AddOPeople1()
        {
            List<People> list = new List<People>();
            for (int i = 0; i < 10000; i++)
            {
                People people = new People
                {
                    PeopleID = Guid.NewGuid(),
                    BankCard = "123",
                    Creator = "1",
                    IDCard = "420114199602191215",
                    Name = "1"
                };
                list.Add(people);
            }
            DateTime now = DateTime.Now;
            _people.Addpeoplelist(list);
            return DateTime.Now - now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddOPeople2")]
        public TimeSpan AddOPeople2()
        {
            List<People> list = new List<People>();
            for (int i = 0; i < 10000; i++)
            {
                People people = new People
                {
                    PeopleID = Guid.NewGuid(),
                    BankCard = "123",
                    Creator = "1",
                    IDCard = "420114199602191215",
                    Name = "1"
                };
                list.Add(people);
            }
            DateTime now = DateTime.Now;
            _people.Addpeoplelistasyn(list);
            return DateTime.Now - now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateOPeople")]
        public bool UpdateOPeople([FromBody]List<People> peoples)
        {
            return _people.UpdateEntity(peoples);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateOPeople2")]
        public bool UpdateOPeople2(List<People> peoples)
        {
            return _people.UpdateEntityasync(peoples);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetPeoples")]
        public string GetPeoples(string id)
        {
            return JsonConvert.SerializeObject(_people.GetPeoples(id));
        }
    }
}