public static class ApiResponse
{
    public static object SuccessResponse(object data)
    {
        return new { result = "Success", data = data };
    }

    public static object ErrorResponse(string message, string errorDetails)
    {
        return new
        {
            result = "Error",
            message = message,
            error = errorDetails,
        };
    }
}
