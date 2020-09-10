using System;
using Newtonsoft.Json;

namespace MindMapWebSocketServer.Utility
{
    public class Validators
    {
        static public bool IsValidJson(string strInput) 
        {
            if (string.IsNullOrEmpty(strInput)) { return false; }
            strInput = strInput.Trim();

            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) ||
                (strInput.StartsWith("[") && strInput.EndsWith("]")))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(strInput);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } 
    }
}
