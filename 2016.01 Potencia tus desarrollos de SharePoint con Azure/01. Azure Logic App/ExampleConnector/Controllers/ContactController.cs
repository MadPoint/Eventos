using ExampleLogicAppConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ExampleLogicAppConnector.Controllers
{
    public class ContactController : ApiController
    {
        private Contact[] _contacts =
        {
            new Contact {Name = "José Carlos", Phone = "123456789", Email="i42roavj@gmail.com" },
            new Contact {Name = "José Carlos II", Phone = "123456789", Email="josecarlo_14y@hotmail.com" }
        };

        public Contact Get(int id)
        {
            return _contacts.ElementAtOrDefault(id);
        }

        public IEnumerable<Contact> Get()
        {
            return _contacts;
        }

    }
}
