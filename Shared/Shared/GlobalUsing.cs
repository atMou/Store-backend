global using System.Diagnostics.CodeAnalysis;
global using System.Text.RegularExpressions;

global using LanguageExt;
global using LanguageExt.Common;
global using LanguageExt.Traits;
global using LanguageExt.Traits.Domain;
global using LanguageExt.UnsafeValueAccess;

global using MediatR;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Options;

global using SendGrid;
global using SendGrid.Helpers.Mail;

global using Shared.Domain.Abstractions;
global using Shared.Domain.Errors;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Email.Options;
global using Shared.Infrastructure.Errors;

global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Formats.Jpeg;
global using SixLabors.ImageSharp.Processing;

global using static LanguageExt.Prelude;

global using Unit = LanguageExt.Unit;
