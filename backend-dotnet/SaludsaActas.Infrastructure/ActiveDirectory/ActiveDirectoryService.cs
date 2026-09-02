using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.Infrastructure.ActiveDirectory;

public class ActiveDirectoryService : IActiveDirectoryService
{
    private readonly ActiveDirectoryOptions _options;

    public ActiveDirectoryService(
        IOptions<ActiveDirectoryOptions> options)
    {
        _options = options.Value;
    }

    public async Task<List<ActiveDirectoryEmployeeDto>> SearchEmployeesAsync(
        string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<ActiveDirectoryEmployeeDto>();
        }

        return await Task.Run(() =>
        {
            using var connection = CreateConnection();

            var escapedTerm =
                EscapeLdapFilterValue(searchTerm.Trim());

            var filter =
                "(&(objectCategory=person)(objectClass=user)" +
                "(|" +
                $"(sAMAccountName=*{escapedTerm}*)" +
                $"(name=*{escapedTerm}*)" +
                $"(displayName=*{escapedTerm}*)" +
                $"(employeeID=*{escapedTerm}*)" +
                $"(mail=*{escapedTerm}*)" +
                "))";

            var request = new SearchRequest(
                _options.BaseDn,
                filter,
                SearchScope.Subtree,
                GetAttributes());

            request.SizeLimit =
                _options.SearchLimit > 0
                    ? _options.SearchLimit
                    : 15;

            var response =
                (SearchResponse)connection.SendRequest(request);

            var employees =
                new List<ActiveDirectoryEmployeeDto>();

            foreach (SearchResultEntry entry in response.Entries)
            {
                var employee = MapEmployee(entry);

                if (string.IsNullOrWhiteSpace(employee.Username))
                {
                    continue;
                }

                employees.Add(employee);
            }

            return employees
                .GroupBy(
                    employee => employee.Username,
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(employee => employee.FullName)
                .ToList();
        });
    }

    public async Task<ActiveDirectoryEmployeeDto?> GetByUsernameAsync(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        return await Task.Run(() =>
        {
            using var connection = CreateConnection();

            var escapedUsername =
                EscapeLdapFilterValue(username.Trim());

            var filter =
                "(&(objectCategory=person)(objectClass=user)" +
                $"(sAMAccountName={escapedUsername}))";

            var request = new SearchRequest(
                _options.BaseDn,
                filter,
                SearchScope.Subtree,
                GetAttributes());

            request.SizeLimit = 1;

            var response =
                (SearchResponse)connection.SendRequest(request);

            if (response.Entries.Count == 0)
            {
                return null;
            }

            return MapEmployee(
                response.Entries[0]);
        });
    }

    private LdapConnection CreateConnection()
    {
        var username =
            Environment.GetEnvironmentVariable(
                "LDAP_USERNAME");

        var password =
            Environment.GetEnvironmentVariable(
                "LDAP_PASSWORD");

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException(
                "La variable LDAP_USERNAME no está configurada.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "La variable LDAP_PASSWORD no está configurada.");
        }

        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException(
                "El Host de Active Directory no está configurado.");
        }

        if (string.IsNullOrWhiteSpace(_options.BaseDn))
        {
            throw new InvalidOperationException(
                "El BaseDn de Active Directory no está configurado.");
        }

        var upn =
            BuildUpn(
                username,
                _options.BaseDn);

        var identifier =
            new LdapDirectoryIdentifier(
                _options.Host,
                _options.Port);

        var credential =
            new NetworkCredential(
                upn,
                password);

        var connection =
            new LdapConnection(
                identifier,
                credential,
                AuthType.Basic);

        connection.SessionOptions.ProtocolVersion = 3;

        connection.SessionOptions.SecureSocketLayer =
            _options.UseSsl;

        connection.Timeout =
            TimeSpan.FromSeconds(15);

        connection.Bind();

        return connection;
    }

    private static string BuildUpn(
        string username,
        string baseDn)
    {
        var cleanUsername =
            username
                .Trim()
                .ToLowerInvariant();

        if (cleanUsername.Contains('@'))
        {
            return cleanUsername;
        }

        var domainParts =
            baseDn
                .Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part =>
                    part.StartsWith(
                        "dc=",
                        StringComparison.OrdinalIgnoreCase))
                .Select(part =>
                    part[3..])
                .Where(part =>
                    !string.IsNullOrWhiteSpace(part))
                .ToList();

        if (domainParts.Count == 0)
        {
            throw new InvalidOperationException(
                "No se pudo obtener el dominio a partir del BaseDn.");
        }

        var domain =
            string.Join(
                ".",
                domainParts);

        return $"{cleanUsername}@{domain}";
    }

    private static string[] GetAttributes()
    {
        return new[]
        {
            "sAMAccountName",
            "name",
            "displayName",
            "employeeID",
            "l",
            "mail",
            "department",
            "description"
        };
    }

    private static ActiveDirectoryEmployeeDto MapEmployee(
        SearchResultEntry entry)
    {
        var fullName =
            GetAttribute(entry, "name");

        if (string.IsNullOrWhiteSpace(fullName))
        {
            fullName =
                GetAttribute(
                    entry,
                    "displayName");
        }

        return new ActiveDirectoryEmployeeDto
        {
            Username =
                GetAttribute(
                    entry,
                    "sAMAccountName"),

            FullName =
                fullName,

            NationalId =
                GetAttribute(
                    entry,
                    "employeeID"),

            City =
                GetAttribute(
                    entry,
                    "l"),

            Email =
                GetAttribute(
                    entry,
                    "mail"),

            Department =
                GetAttribute(
                    entry,
                    "department"),

            Position =
                GetAttribute(
                    entry,
                    "description")
        };
    }

    private static string GetAttribute(
        SearchResultEntry entry,
        string attributeName)
    {
        if (!entry.Attributes.Contains(attributeName))
        {
            return string.Empty;
        }

        var attribute =
            entry.Attributes[attributeName];

        if (attribute is null ||
            attribute.Count == 0)
        {
            return string.Empty;
        }

        var value =
            attribute[0];

        if (value is byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        return value?.ToString()
               ?? string.Empty;
    }

    private static string EscapeLdapFilterValue(
        string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var builder =
            new StringBuilder();

        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    builder.Append(@"\5c");
                    break;

                case '*':
                    builder.Append(@"\2a");
                    break;

                case '(':
                    builder.Append(@"\28");
                    break;

                case ')':
                    builder.Append(@"\29");
                    break;

                case '\0':
                    builder.Append(@"\00");
                    break;

                default:
                    builder.Append(character);
                    break;
            }
        }

        return builder.ToString();
    }
}