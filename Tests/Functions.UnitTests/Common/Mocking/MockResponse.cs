using System;
using System.Diagnostics.CodeAnalysis;
using Azure;

namespace Functions.UnitTests.Common.Mocking;

[ExcludeFromCodeCoverage]
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