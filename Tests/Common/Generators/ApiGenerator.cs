using Common.Extensions;

namespace Common.Generators;

public static class ApiGenerator
{
    public static string GenerateApiMethod()
    {
        return Methods.SelectRandom();
    }

    public static int GenerateStatusCode()
    {
        return StatusCodes.SelectRandom();
    }

    public static readonly string[] Methods = new[]
    {
        "GET",
        "POST",
        "PUT",
        "PATCH",
        "DELETE"
    };

    public static readonly int[] StatusCodes = new[]
    {
        200,
        204,
        400,
        401,
        403,
        404,
        409,
        500
    };
}