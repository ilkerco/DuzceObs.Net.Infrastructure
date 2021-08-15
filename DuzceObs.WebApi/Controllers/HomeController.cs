using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DuzceObs.Core.Model.Entities;
using DuzceObs.WebApi.Dto;
using DuzceObs.WebApi.Helpers;
using DuzceObs.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DuzceObs.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class HomeController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IAuthHelper _authHelper;
        private readonly IDuzceObsDataService _duzceObsDataService;
        private readonly IMapper _mapper;
        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthHelper authHelper,
            IMapper mapper,
            IUserService userService,
            IDuzceObsDataService duzceObsDataService
            )
        {
            _duzceObsDataService = duzceObsDataService;
            _authHelper = authHelper;
            _userManager = userManager;
            _mapper = mapper;
            _userService = userService;
        }
        [HttpGet("getCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId =  _userService.GetCurrentUser();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("There is no user");
                }
                var appUser = await _userManager.Users.FirstOrDefaultAsync(
                    u => u.Email == user.Email);
                if (appUser.UserType == "Student")
                {
                    var userToReturn = _mapper.Map<StudentDto>(appUser);
                    return Ok(new
                    {
                        token = _authHelper.GenerateJwtToken(appUser).Result,
                        user = userToReturn,
                    });
                }
                else
                {
                    var userToReturn = _mapper.Map<InstructorDto>(appUser);
                    return Ok(new
                    {
                        token = _authHelper.GenerateJwtToken(appUser).Result,
                        user = userToReturn,
                    });
                }

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("getALlStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var _students  = await _userManager.Users.Where(x =>x.UserType=="Student").ToListAsync();
                var deneme = _students.Cast<Student>().ToList();
                var studentList = _mapper.Map<List<Student>>(deneme);
                return Ok(studentList);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("getAllStudentsWithGrades")]
        public async Task<IActionResult> GetDersWithGrades()
        {
            try
            {
                var _students = await _duzceObsDataService.GetDersResponseWithGrades();
                return Ok(_students);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("getStudentsDers")]
        public async Task<IActionResult> GetStudentsDers()
        {
            try
            {
                var _students = await _duzceObsDataService.GetStudentDersWithGrades();
                return Ok(_students);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("getAllDersByInstructor")]
        public async Task<IActionResult> GetAllDersByInstructor()
        {
            try
            {
                var data = await _duzceObsDataService.GetAllDersByInstructor();
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("getAllDersByStudent")]
        public async Task<IActionResult> GetAllDersByStudent()
        {
            try
            {
                var data = await _duzceObsDataService.GetAllDersByStudent();
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost("addDers")]
        public async Task<IActionResult> AddDers([FromBody] AddDersDto dersDto)
        {
            try
            {
                var success = await _duzceObsDataService.AddDers(dersDto);
                if (success)
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("addDersToOgrenci/{dersId}/{ogrenciId}")]
        public async Task<IActionResult> AddDersToOgrenci([FromRoute]int dersId, [FromRoute] string ogrenciId)
        {
            try
            {
                var success = await _duzceObsDataService.AddDersToOgrenci(dersId,ogrenciId);
                if (success)
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("deleteDersFromOgrenci/{dersId}/{ogrenciId}")]
        public async Task<IActionResult> DeleteDersFromOgrenci([FromRoute] int dersId, [FromRoute] string ogrenciId)
        {
            try
            {
                var success = await _duzceObsDataService.DeleteDersFromOgrenci(dersId, ogrenciId);
                if (success)
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [AllowAnonymous]
        [HttpGet("getExcelTemplate/{dersId}")]
        public async Task<IActionResult> DownloadExcelDocument([FromRoute] int dersId)
        {
            var data = await _duzceObsDataService.GetSingleDersResponseWithGrades(dersId);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = data.DersKodu+"_"+data.DersAdi+"_notlar.xlsx";
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    int columnCount = 0;
                    IXLWorksheet worksheet =
                    workbook.Worksheets.Add(data.DersKodu+"_");
                    worksheet.Cell(1, 1).Value = "FirstName";
                    worksheet.Cell(1, 2).Value = "LastName";
                    worksheet.Cell(1, 3).Value = "Ogrenci No";
                    worksheet.Cell(1, 4).Value = "Ders Kodu";
                    worksheet.Cell(1, 5).Value = "Ders Adi";
                    for(int i = 0; i < data.DersKriters.Count; i++)
                    {
                        worksheet.Cell(1, i+6).Value = data.DersKriters[i].DersDegerlendirmeName + "(%"
                            + data.DersKriters[i].Yuzde + ")";
                        columnCount = i;
                    }
                    columnCount = columnCount + 1 + 5;
                    for (int j = 0; j < data.Students.Count; j++)
                    {
                        worksheet.Cell(j+2, 1).Value = data.Students[j].FirstName;
                        worksheet.Cell(j+2, 2).Value = data.Students[j].LastName;
                        worksheet.Cell(j+2, 3).Value = data.Students[j].OgrNo;
                        worksheet.Cell(j + 2, 3).DataType = XLDataType.Number;
                        worksheet.Cell(j+2, 4).Value = data.DersKodu;
                        worksheet.Cell(j+2, 5).Value = data.DersAdi;
                        
                        
                        for(int columIndex = 6; columIndex <= columnCount; columIndex++)
                        {
                            var gg = worksheet.Cell(1, columIndex).Value.ToString();
                            var startIndex = worksheet.Cell(1, columIndex).Value.ToString().IndexOf('(');
                            gg = gg.Remove(startIndex,gg.Length-startIndex);
                            for (int t = 0; t < data.Students[j].Notlar.Count; t++)
                            {
                                if (gg == data.Students[j].Notlar[t].DersDegerlendirmeName)
                                {
                                    worksheet.Cell(j + 2, columIndex).Value = data.Students[j].Notlar[t].Not;
                                }
                            }
                                
                        }
                            
                            
                            //data.Students[j].Notlar[t].DersDegerlendirmeName;
                        
                    }

                    worksheet.Columns().AdjustToContents();
                    
                    
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [AllowAnonymous]
        [HttpPost("uploadExcelTemplate")]
        public async Task<IActionResult> UploadExcelDocument(IFormFile file)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                if (file == null || file.Length == 0)
                    return BadRequest("File Not Selected");

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    return BadRequest("File Not Selected");
                Dictionary<string, double> ogr_NotDict = new Dictionary<string, double>();
               
                var ogrList = new List<AddNotDto>();
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    using (XLWorkbook workbook = new XLWorkbook(ms))
                    {
                        var worksheet = workbook.Worksheet(1);
                        int anan = worksheet.Columns().Count();
                        for (int i = 1; i <= 5; i++)
                        {
                            dt.Columns.Add(worksheet.Cell(1, i).Value.ToString());
                        }
                        int n = worksheet.Rows().Count();
                        for (int i = 2; i <= n; i++)
                        {
                            if (worksheet.Cell(i, 1).Value.ToString().Length <= 0)
                            {
                                continue;
                            }
                            DataRow dr = dt.NewRow();

                            //int j, k = worksheet.Columns().Count();
                            for (int j = 1; j <= 5; j++)
                            {
                                // i= satır index, j=sütun index, closedXML worksheet için indexler 1'den başlıyor, ama datatable için 0'dan başladığı için j-1 diyoruz
                                dr[j - 1] = worksheet.Cell(i, j).Value;
                            }
                            for (int j = 6; j <= worksheet.Columns().Count(); j++)
                            {
                                if(worksheet.Cell(1, j).Value.ToString().Length > 0)
                                {
                                    var gg = worksheet.Cell(1, j).Value.ToString();
                                    var startIndex = worksheet.Cell(1, j).Value.ToString().IndexOf('(');
                                    gg = gg.Remove(startIndex, gg.Length - startIndex);
                                    ogr_NotDict.Add(gg.ToString(), Double.Parse(worksheet.Cell(i, j).Value.ToString()));
                                }
                                
                            }
                            Dictionary<string, double> ogr_NotDict2 = new Dictionary<string, double>();
                            foreach (KeyValuePair<string, double> entry in ogr_NotDict)
                            {
                                ogr_NotDict2.Add(entry.Key, entry.Value);
                                // do something with entry.Value or entry.Key
                            }
                            dt.Rows.Add(dr);
                            ogrList.Add(new AddNotDto
                            {
                                DersAdi = dt.Rows[0][4].ToString(),
                                DersKodu = dt.Rows[0][3].ToString(),
                                FirstName = dt.Rows[0][0].ToString(),
                                LastName = dt.Rows[0][1].ToString(),
                                OgrenciNo = dt.Rows[0][2].ToString(),
                                Notlar = ogr_NotDict2

                            });
                            dt.Rows.Clear();
                            ogr_NotDict.Clear();
                            
                        }
                        
                    }
                }
                var isSuccess = await _duzceObsDataService.AddNotFromExcel(ogrList);
                //var rowCount = dt.Rows.Count;
                //string json = DataTableToJSONWithStringBuilder(dt);
                if (!isSuccess)
                {
                    return BadRequest();
                }

                return Ok(isSuccess);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }
        public static string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

    }
}
