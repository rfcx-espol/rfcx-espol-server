using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Services;
using WebApplication.Models;
using WebApplication.IRepository;
using WebApplication.Repository;
using System;

namespace WebApplication.Controllers
{
  [Authorize]
    //[ApiController]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("[controller]/index")]
        public IActionResult Index()
        {
            ViewBag.UsersInDB =  _userRepository.GetAll();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("[controller]/update/{userId}")]
        public IActionResult Update(string userId)
        {
            ViewBag.UserToEdit = _userRepository.GetById(userId);
            return View();
        }

        [HttpPost("api/[controller]/update/{userId}")]
        public IActionResult Update(string userId, [FromBody]User user)
        {
            user.UserId = userId;
            Console.WriteLine(userId);
            var updated = _userRepository.UpdateById(userId, user);
            Console.WriteLine(updated);
            Console.WriteLine(user.Username);
            if (!updated)
            {
                return BadRequest(new { message = "User not updated" });
                
            }
            user.Password = null;
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Create(string Username, string Password, string Role, string Name) 
        {
            var user = new User();
            user.Username = Username; user.Password = Password; user.Role = Role; user.Name = Name;
            _userRepository.AddAsync(user);
            return Redirect("/users/index");
        }

        [HttpPost("api/[controller]/new")]
        public IActionResult New([FromBody]User user)
        {
            Console.WriteLine(user.Username);
            return Ok(_userRepository.AddAsync(user));
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userRepository.GetAll();
            return Ok(users);
        }

        [HttpDelete("api/[controller]/{userId}")]
        public bool Delete(string userId)
        {
            
            bool valor = _userRepository.Remove(userId);
            return valor;
        }
    }
}
