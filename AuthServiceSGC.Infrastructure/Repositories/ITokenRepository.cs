﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public interface ITokenRepository
    {
        public Task AddTokenToBlacklist(string token);
    }
}
