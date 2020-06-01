using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public enum OperationType
    {
        Insert,
        InsertOrUpdate,
        InsertOrUpdateDelete,
        Update,
        Delete,
        Read
    }
}
