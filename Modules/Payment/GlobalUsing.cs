global using LanguageExt;

global using MediatR;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Payment.Persistence.Data;

global using Serilog;

global using Shared.Application.Abstractions;
global using Shared.Domain.Abstractions;
global using Shared.Domain.Errors;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Clock;
global using Shared.Persistence.Db.Monad;
global using Shared.Persistence.Interceptors;

global using static LanguageExt.Prelude;
global using static Shared.Persistence.Db.Monad.Db;

global using Unit = LanguageExt.Unit;
