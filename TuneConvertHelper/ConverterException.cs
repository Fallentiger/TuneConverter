using System;
using System.Collections.Generic;
using System.Text;

namespace TuneConvertHelper
{
    public class ConverterException : Exception
    {
        public ConverterException(string msg) : base(msg) { }
    }
}
