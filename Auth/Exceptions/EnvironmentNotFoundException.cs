namespace Auth.Exceptions;

public class EnvironmentNotFoundException: Exception 
{
    public EnvironmentNotFoundException(string env): base($"{env} not found")
    {

    }
} 
