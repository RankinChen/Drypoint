﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseModels
{
    [Serializable]
    public class ErrorInfo
    {
        public int Code { get; set; }
        
        public string Message { get; set; }
        
        public string Details { get; set; }
        
        public ValidationErrorInfo[] ValidationErrors { get; set; }


        public ErrorInfo()
        {

        }

        public ErrorInfo(string message)
        {
            Message = message;
        }


        public ErrorInfo(int code)
        {
            Code = code;
        }

        public ErrorInfo(int code, string message)
            : this(message)
        {
            Code = code;
        }

        public ErrorInfo(string message, string details)
            : this(message)
        {
            Details = details;
        }

        public ErrorInfo(int code, string message, string details)
            : this(message, details)
        {
            Code = code;
        }
    }
}
