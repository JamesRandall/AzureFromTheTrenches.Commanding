using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("hello world");
        }
    }
}
