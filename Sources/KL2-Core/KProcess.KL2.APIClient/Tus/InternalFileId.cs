﻿using System;
using tusdotnet.Models;

namespace KProcess.KL2.APIClient.Tus
{
    sealed class InternalFileId
    {
        public string FileId { get; set; }

        public InternalFileId()
        {
            FileId = Guid.NewGuid().ToString("n");
        }

        public InternalFileId(string fileId)
        {
            if (!Guid.TryParseExact(fileId, "n", out var _))
                throw new TusStoreException("Invalid file id");

            FileId = fileId;
        }

        public override string ToString() =>
            FileId;
    }
}
