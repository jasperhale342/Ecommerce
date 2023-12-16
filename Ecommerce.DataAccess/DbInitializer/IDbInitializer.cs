using Ecommerce.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {

       

        void Initialze();
    }
}
