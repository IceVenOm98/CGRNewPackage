using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CgrNewPackage
{
    public interface ICgrSmsSender
    {
        int SendSms(string messageAndNumber);
    }
}
