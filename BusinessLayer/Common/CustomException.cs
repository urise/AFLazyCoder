using System;

namespace BusinessLayer.Common
{
    public class CustomException: Exception
    {
        public CustomException(string message): base(message){}
    }
}
