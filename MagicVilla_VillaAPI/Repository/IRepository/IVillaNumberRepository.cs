﻿using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
       
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
        

    }
}
