using System;

class ConfigurationException :
    Exception
{
    public ConfigurationException(string message)
        : base(message)
    {
    }
}