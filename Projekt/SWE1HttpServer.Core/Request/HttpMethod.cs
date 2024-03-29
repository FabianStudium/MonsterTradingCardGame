﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1HttpServer.Core.Request
{
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }

    public static class MethodUtilities
    {
        public static HttpMethod GetMethod(string method)
        {
            method = method.ToLower();
            HttpMethod parsedMethod = method switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                "patch" => HttpMethod.Patch,
                _ => HttpMethod.Get
            };

            return parsedMethod;
        }
    }
}
