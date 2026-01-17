global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.RegularExpressions;

global using Db.Errors;

global using LanguageExt;
global using LanguageExt.Common;
global using LanguageExt.Traits;
global using LanguageExt.Traits.Domain;
global using LanguageExt.UnsafeValueAccess;

global using MediatR;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

global using SendGrid;
global using SendGrid.Helpers.Mail;

global using Shared.Application.Abstractions;
global using Shared.Application.Behaviour;
global using Shared.Application.Contracts.Order.Results;
global using Shared.Domain.Abstractions;
global using Shared.Domain.Errors;
global using Shared.Domain.Validations;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Email.Options;
global using Shared.Infrastructure.Errors;
global using Shared.Persistence.Db.Monad;
global using Shared.Persistence.Extensions;

global using SixLabors.ImageSharp;

global using static LanguageExt.Prelude;

global using Unit = LanguageExt.Unit;
