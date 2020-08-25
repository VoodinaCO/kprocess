using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Security.Activation
{
    [DataContract]
    public class UsersPool : HashSet<int>
    {
    }
}
