using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


static class TypeConverter
{
    /*
    public static int ConvertToInt(object input)
    {
        try
        {
            return Convert.ToInt32(input);
        }
        finally
        {
            input.
        }
    }
    */

    public static class Json
    {
        public static int ConvertToInt(JsonElement input)
        {

            int result;

            result = input.GetInt32();
            
            return result;
        }

        public static int ConvertToInt(object input)
        {
            int result;

            if (input is not JsonElement element)
            {
                throw new Exception("Object to convert isn't of type JsonElement");
            }

            result = element.GetInt32();

            return result;
        }
    }
}

