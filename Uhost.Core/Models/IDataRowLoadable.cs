using System.Data;

namespace Uhost.Core.Models
{
    public interface IDataRowLoadable
    {
        void LoadFromDataRow(DataRow row);
    }
}
