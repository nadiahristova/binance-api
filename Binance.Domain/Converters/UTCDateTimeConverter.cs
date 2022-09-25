﻿using Newtonsoft.Json;

namespace Binance.Domain.Converters;

public class UTCDateTimeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
