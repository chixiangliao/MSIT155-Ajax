using AjaxDemoV2.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjAjaxHomework.Models;
using System.Linq;
using System.Text;

namespace prjAjaxHomework.Controllers
{
    public class ApiController : Controller
    {
        private MyDBContext _dbContext;
        private IWebHostEnvironment _environment;

        public ApiController(MyDBContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
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
        public IActionResult Register(Member member, IFormFile avatar)
        {
            if (string.IsNullOrEmpty(member.Name))
            {
                member.Name = "guest";
            }

            string fileName = "empty.jpg";
            if (avatar != null)
            {
                fileName = avatar.FileName;
            }

            //新增資料置資料庫
            if (_dbContext.Members.Select(f=>f.FileName).Contains(fileName))
            {
                return Content("檔名重複!!, 請跟改");
            }

            member.FileName = fileName;

            //圖片轉二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                avatar?.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }
            member.FileData = imgByte;

            //存入資料庫
            _dbContext.Members.Add(member);
            _dbContext.SaveChanges();

            //獲取網頁跟根目錄及生成指定資料夾路徑
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads/images", fileName);
            //存入資料至指定資料夾
            using (var filestream = new FileStream(uploadPath, FileMode.Create))
            {
          
                filestream.CopyTo(filestream);
            }

            

            return Content($"嗨{member.Name}， 年齡: {member.Age}， 電子郵件: {member.Email} 帳號儲存成功", "text/plain", Encoding.UTF8);
            
        }

        [HttpPost]
        public IActionResult Message(UserInfoDTO userDTO)
        {

            // return Content($"{userDTO.Name}, {userDTO.Email}, {userDTO.Age} 123", "text/plain", Encoding.UTF8);
            return Content($"嗨{userDTO.Name}， 年齡: {userDTO.Age}， 電子郵件: {userDTO.Email}", "text/plain", Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO search)
        {
            var spots = search.CategoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == search.CategoryId);

            //keyword搜尋
            if (!String.IsNullOrEmpty(search.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(search.keyword) || s.SpotDescription.Contains(search.keyword));
            }

            //排序
            switch (search.sortBy)
            {
                case "spotTitle":
                    spots = search.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) :
                                                spots.OrderByDescending(s => s.SpotTitle);
                    break;

                case "spotId":
                    spots = search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) :
                                                spots.OrderByDescending(s => s.SpotId);
                    break;

                default:
                    spots = search.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) :
                                               spots.OrderByDescending(s => s.CategoryId);
                    break;
            }

            //分頁
            int totalCount = spots.Count();
            int pageSize = search.pageSize ?? 9;
            int page = search.page ?? 1;
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            spots = spots.Skip((int)(page - 1) * pageSize).Take(pageSize);

            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = totalPages;
            spotsPaging.TotalCount = totalCount;
            spotsPaging.SpotsResult = spots.ToList();

            return Json(spotsPaging);
        }
    }
}
