using System;
using System.Collections.Generic;
using System.Linq;
using MyLink.Models;

namespace MyLink.Classes
{
    public class CombosHelper : IDisposable
    {
        private static MyLinkContext db = new MyLinkContext();

        public static List<Department> GetDepartments()
        {
            var department = db.Departments.ToList();
            department.Add(new Department { DepartmentId = 0, Name = "[Select a department...]" });
            return department.OrderBy(d => d.Name).ToList();
        }

        public static List<City> GetCities(int departmentId)
        {
            var city = db.Cities.Where(c => c.DepartmentId == departmentId).ToList();
            city.Add(new City { CityId = 0, Name = "[Select a city...]" });
            return city.OrderBy(d => d.Name).ToList();
        }        

        public static List<UserRol> GetUserRols()
        {
            var userRol = db.UserRols.ToList();
            userRol.Add(new UserRol { UserRolId = 0, Name = "[Seleciona un Rol...]" });
            return userRol.OrderBy(d => d.Name).ToList();
        }              
        
        public void Dispose()
        {
            db.Dispose();
        }        
    }
}