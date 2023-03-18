using KEDI.Core.Premise.Models.Sync;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace KEDI.Core.Premise.AppContext.Extensions
{
    public static class DbContextExtensions
    {
        public static IQueryable<object> Set(this DbContext dbContext, Type entityType)
        {
            return (IQueryable<object>)dbContext.GetType().GetMethod("Set")
                .MakeGenericMethod(entityType).Invoke(dbContext, null);
        }
    }
}
