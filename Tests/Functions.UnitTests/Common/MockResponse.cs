using Azure;
using System;

namespace Functions.UnitTests.Common;

public class MockResponse<T> : Response<T>
{
    private readonly T _value;

    public MockResponse(T value)
    {
        _value = value;
    }

    public override T Value => _value;

    public override Response GetRawResponse()
    {
        throw new NotImplementedException();
    }
}