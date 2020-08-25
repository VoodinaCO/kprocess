namespace KProcess.Ksmed.Models
{
    public partial class ActionPredecessorSuccessor
    {
        public int ActionPredecessorId { get; set; }
        public int ActionSuccessorId { get; set; }

        public KAction Predecessor { get; set; }
        public KAction Successor { get; set; }
    }
}
