using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringUtils
{
    /// <summary>
    /// Returns a lowercase version of the input string, with whitespaces replaced with underscores.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string KeyFriendlyString(string input)
    {
        return input.Replace(' ', '_').ToLowerInvariant();
    }
   
}
