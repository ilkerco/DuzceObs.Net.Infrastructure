using DuzceObs.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Services.Interfaces
{
    public interface IAuthHelper
    {
        Task<string> GenerateJwtToken(User user);
    }
}
