using System;

namespace Kprocess.KL2.FileServer
{
    public static class Bits
    {
        // BITS Protocol header keys
        public const string K_BITS_SESSION_ID = "BITS-Session-Id";
        public const string K_BITS_ERROR_CONTEXT = "BITS-Error-Context";
        public const string K_BITS_ERROR_CODE = "BITS-Error-Code";
        public const string K_BITS_PACKET_TYPE = "BITS-Packet-Type";
        public const string K_BITS_SUPPORTED_PROTOCOLS = "BITS-Supported-Protocols";
        public const string K_BITS_PROTOCOL = "BITS-Protocol";

        // HTTP Protocol header keys
        public const string K_ACCEPT_ENCODING = "Accept-Encoding";
        public const string K_CONTENT_NAME = "Content-Name";
        public const string K_CONTENT_LENGTH = "Content-Length";
        public const string K_CONTENT_RANGE = "Content-Range";
        public const string K_CONTENT_ENCODING = "Content-Encoding";

        // BITS Protocol header values
        public const string V_ACK = "Ack";

        // BITS server errors
        public enum BITSServerHResult : uint
        {
            // default context
            BG_ERROR_CONTEXT_REMOTE_FILE = 0x5,
            // official error codes
            BG_E_TOO_LARGE = 0x80200020,
            E_INVALIDARG = 0x80070057,
            E_ACCESSDENIED = 0x80070005,
            ZERO = 0x0,  // protocol specification does not give a name for this HRESULT
                         // custom error code
            ERROR_CODE_GENERIC = 0x1
        }

        public class BITSServerException : Exception
        {
            public BITSServerException(string message) : base(message)
            { }
            public BITSServerException(string message, Exception innerException) : base(message, innerException)
            { }
        }

        public class ClientProtocolNotSupported : BITSServerException
        {
            public Guid RequestProtocol { get; }

            public ClientProtocolNotSupported(Guid protocol) : base("Server supports neither of the requested protocol versions")
            {
                RequestProtocol = protocol;
            }
        }

        public class ServerInternalError : BITSServerException
        {
            public ServerInternalError(Exception exception) : base("Internal server error encountered", exception)
            { }
        }

        public class InvalidFragment : BITSServerException
        {
            public long LastRangeEnd { get; }
            public long NewRangeStart { get; }

            public InvalidFragment(long last_range_end, long new_range_start) : base("Invalid fragment received on server")
            {
                LastRangeEnd = last_range_end;
                NewRangeStart = new_range_start;
            }
        }

        public class FragmentTooLarge : BITSServerException
        {
            public long FragmentSize { get; }

            public FragmentTooLarge(long fragment_size) : base("Oversized fragment received on server")
            {
                FragmentSize = fragment_size;
            }
        }

        public class UploadAccessDenied : BITSServerException
        {
            public UploadAccessDenied() : base("Write access to requested file upload is denied")
            { }
        }
    }
}
