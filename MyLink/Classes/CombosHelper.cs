﻿using System;
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

        public static List<Bank> GetBanks()
        {
            var bank = db.Banks.ToList();
            bank.Add(new Bank { BankId = 0, Name = "[Select a Bank...]" });
            return bank.OrderBy(d => d.Name).ToList();
        }

        public static List<Project> GetProjects()
        {
            var project = db.Projects.ToList();
            project.Add(new Project { ProjectId = 0, Name = "[Seleccionar Proyecto...]" });
            return project.OrderBy(d => d.Name).ToList();
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

        public static List<Language> GetLanguages()
        {
            var language = db.Languages.ToList();
            language.Add(new Language { LanguageId = 0, Name = "[Seleciona un Idioma...]" });
            return language.OrderBy(d => d.Name).ToList();
        }

        public static List<LinkCategory> GetLinkCategory()
        {
            var linkCategorie = db.LinkCategories.ToList();
            linkCategorie.Add(new LinkCategory { LinkCategoryId = 0, Name = "[Seleciona una Categoria...]" });
            return linkCategorie.OrderBy(d => d.Name).ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }        
    }
}