using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KEDI.Core.Premise.Models.Files
{
    public class FileCollection<TEntity>
    {
        public IFormFileCollection Files { set; get; }
        public TEntity[] Items { set; get; }
    }
}