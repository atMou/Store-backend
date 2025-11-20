global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using System.Threading.Tasks;

global using LanguageExt;
global using LanguageExt.Common;

global using MediatR;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Order.Domain.Enums;
global using Order.Domain.Models;
global using Order.Domain.ValueObjects;
global using Order.Persistence.Data;

global using Serilog;

global using Shared.Application.Abstractions;
global using Shared.Application.Behaviour;
//global using MediatR;

global using Shared.Domain.Abstractions;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Clock;
global using Shared.Persistence.Db.Monad;
global using Shared.Persistence.Interceptors;

global using static LanguageExt.Prelude;
