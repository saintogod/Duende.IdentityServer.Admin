﻿using System.Collections.Generic;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseUserDto<TUserId> : IBaseUserDto
    {
        public TUserId Id { get; set; }

        public bool IsDefaultId() => EqualityComparer<TUserId>.Default.Equals(Id, default(TUserId));

        object IBaseUserDto.Id => Id;
    }
}