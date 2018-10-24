﻿using System;
using MyLink.Models;

namespace MyLink.Classes
{
    public class DBHelper
    {
        public static Response SaveChanges(MyLinkContext db)
        {
            try
            {
                db.SaveChanges();
                return new Response
                {
                    Succeeded = true,
                };
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    Succeeded = false,
                };
                if (ex.InnerException != null && 
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("_Index"))
                {
                    response.Message = "There are a record with the same value.";
                }
                else if (ex.InnerException != null && 
                         ex.InnerException.InnerException != null &&
                         ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    response.Message = "The record can't be delete because it has related records.";
                }
                else
                {
                    response.Message = ex.Message;
                }

                return response;
            }
        }        
    }
}