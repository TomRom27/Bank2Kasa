using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank2Kasa.Service
{
    public interface IOperationTypeProvider
    {
        string GetOperationTypeName(string operationTypeCode);
        bool GetOperationIncome(string operationTypeCode);
    }
}
