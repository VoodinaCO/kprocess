using PowerArgs;
using System;
using System.Text.RegularExpressions;

namespace CreateReleasePackage
{
    public class VersionReviver
    {
        // By default, PowerArgs does not know what a 'Version' is.  So it will 
        // automatically search your assembly for arg revivers that meet the 
        // following criteria: 
        //
        //    - Have an [ArgReviver] attribute
        //    - Are a public, static method
        //    - Accepts exactly two string parameters
        //    - The return value matches the type that is needed

        // This ArgReviver matches the criteria for a "Version" reviver
        // so it will be called when PowerArgs finds any Version argument.
        //
        // ArgRevivers should throw ArgException with a friendly message
        // if the string could not be revived due to user error.

        [ArgReviver]
        public static Version Revive(string key, string val)
        {
            var match = Regex.Match(val, @"\w+.\w+.\w+.\w+");
            if (match.Success == false)
            {
                throw new ArgException("Not a valid version: " + val);
            }
            else
            {
                return new Version(val);
            }
        }
    }
}
