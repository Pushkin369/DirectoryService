using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryService.Domain.Extensions
{
    public static class GuidExtensions
{
    /// <summary>
    /// Проверяет, что Guid не равен Empty, и возвращает его для цепочек вызовов.
    /// </summary>
    /// <param name="value">Проверяемый Guid.</param>
    /// <param name="paramName">Имя параметра (для сообщения об ошибке).</param>
    /// <exception cref="ArgumentException">Если value == Guid.Empty.</exception>
    public static Guid EnsureNotEmpty(this Guid value, string paramName)
    {
        if (value == Guid.Empty)
        {
            paramName ??= nameof(value); // если не передано, используем "value"
            throw new ArgumentException($"{paramName} cannot be Guid.Empty.", paramName);
        }
        return value;
    }
}
}