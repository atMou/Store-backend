global using Inventory.Domain.Contracts;
global using Inventory.Persistence;

global using LanguageExt;

global using MassTransit;

global using Microsoft.Extensions.Logging;

global using Shared.Application.Abstractions;
global using Shared.Application.Features.Inventory.Events;
global using Shared.Application.Features.Order.Events;
//global using MediatR;

global using Shared.Domain.Abstractions;
global using Shared.Domain.Errors;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Logging;
global using Shared.Persistence.Db.Monad;

global using static LanguageExt.Prelude;
global using static Shared.Persistence.Db.Monad.Db;

global using Unit = LanguageExt.Unit;
