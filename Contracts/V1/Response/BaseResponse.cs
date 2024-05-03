using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.V1.Response
{
    public class BaseResponse
    {
         public List<string> Errors { get; set; }
         public bool IsSuccess => Errors==null || Errors.Count()==0;
    }
}
