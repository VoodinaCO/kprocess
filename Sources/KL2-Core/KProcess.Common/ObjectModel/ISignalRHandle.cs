using System.Threading.Tasks;

namespace KProcess
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISignalRHandle
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public interface ISignalRHandle<in TArgs> : ISignalRHandle
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agrs"></param>
        /// <returns></returns>
        Task SignalRHandler(TArgs agrs);
    }
}
