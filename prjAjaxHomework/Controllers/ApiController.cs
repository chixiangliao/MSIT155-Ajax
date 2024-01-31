using AjaxDemoV2.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using prjAjaxHomework.Models;
using System.Text;

namespace prjAjaxHomework.Controllers
{
    public class ApiController : Controller
    {
        private MyDBContext _dbContext;
        public ApiController(MyDBContext dbContext) 
        { 
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckAccount(UserInfoDTO userDTO)
        {
            if (_dbContext.Members.FirstOrDefault(d => d.Name == userDTO.Name) != null)
            {
                return Content("帳號已存在", "text/plain", Encoding.UTF8);
            }
            return Content("帳號可使用", "text/plain", Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult Message(UserInfoDTO userDTO)
        {

            // return Content($"{userDTO.Name}, {userDTO.Email}, {userDTO.Age} 123", "text/plain", Encoding.UTF8);
            return Content($"嗨{userDTO.Name}， 年齡: {userDTO.Age}， 電子郵件: {userDTO.Email}", "text/plain", Encoding.UTF8);
        }
    }
}
