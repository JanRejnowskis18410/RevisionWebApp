using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyWebApp.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=s18410;Integrated Security=True";
        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var list = new List<Student>();
            using(var con = new SqlConnection(ConnectionString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select FirstName, LastName, BirthDate, Name FROM Student JOIN Enrollment ON Student.IdEnrollment = Enrollment.IdEnrollment JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy;";
                
                con.Open();
                var dr = com.ExecuteReader();
                while(dr.Read())
                {
                    var st = new Student();
                    st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.StudiesName = dr["Name"].ToString();
                    list.Add(st);
                }
            }
            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var list = new List<Student>();
            using (var con = new SqlConnection(ConnectionString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Student where IndexNumber=@index";
                com.Parameters.AddWithValue("index", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    list.Add(st);
                    return Ok(list);
                }
            }
            return NotFound();
        }

        [HttpGet("enrollments/{indexNumber}")]
        public IActionResult GetStudentEnrollment(string indexNumber)
        {
            var list = new List<Enrollment>();
            using(var con = new SqlConnection(ConnectionString))
            using(var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Enrollment.IdEnrollment, Semester, IdStudy, StartDate FROM Enrollment JOIN Student ON Student.IdEnrollment = Enrollment.IdEnrollment WHERE IndexNumber=@indexNumber";
                com.Parameters.AddWithValue("indexNumber", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                    enrollment.IdStudy = Int32.Parse(dr["IdStudy"].ToString());
                    enrollment.Semester = Int32.Parse(dr["Semester"].ToString());
                    enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());

                    list.Add(enrollment);
                }
            }
            if (list.Count > 0)
                return Ok(list);
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to database
            //... generating indexNumber
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent()
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent()
        {
            return Ok("Usuwanie dokończone");
        }
    }
}