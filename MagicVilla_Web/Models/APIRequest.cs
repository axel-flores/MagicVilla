﻿using Microsoft.AspNetCore.Mvc;
using static MagicVilla_Utility.StaticDetails;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string ApiUrl { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
