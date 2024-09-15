﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One.Models.Users.Payloads
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
        public ApiError Error { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
