namespace FreshDeskLib
{
    public enum TicketStatus { Open = 2, Pending = 3, Resolved = 5, Closed = 5 }

    public enum TicketPriority { Low = 1, Medium = 2, High = 3, Urgent = 4 }

    public enum TicketSource { Email = 1, Portal = 2, Phone = 3, Chat = 7, Mobihelp = 8, FeedbackWidget = 9, OutboundEmail = 10 }
}
